﻿using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Secutiry.Cryptography;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Domain.Services.ServiceBus;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.DataAccess.Repositories;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Security.Cryptography;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator;
using MyRecipeBook.Infrastructure.Security.Tokens.Access.Validator;
using MyRecipeBook.Infrastructure.Services.LoggedUser;
using MyRecipeBook.Infrastructure.Services.OpenAI;
using MyRecipeBook.Infrastructure.Services.ServiceBus;
using MyRecipeBook.Infrastructure.Services.Storage;
using OpenAI.Chat;
using System.Reflection;

namespace MyRecipeBook.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddUnitOfWork(services);
            AddRepositories(services);
            AddLoggedUser(services);
            AddTokens(services, configuration);
            AddPasswordEncrypter(services, configuration);
            AddOpenAI(services, configuration);
            AddAzureStorage(services, configuration);
            

            if (configuration.IsUnitTestEnviroment())
                return;

            AddQueue(services, configuration);
            AddDbContext(services, configuration);
            AddFluentMigrator(services, configuration);
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 39));

            services.AddDbContext<MyRecipeBookDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
            });
        }
        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            services.AddScoped<IUserDeleteOnlyRepository, UserRepository>();
            services.AddScoped<IRecipeWriteOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeReadOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeUpdateOnlyRepository, RecipeRepository>();
        }
        private static void AddUnitOfWork(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        private static void AddFluentMigrator(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options.AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
            });
        }
        private static void AddTokens(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

            services.AddScoped<IAccessTokenGenerator>(options => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
            services.AddScoped<IAccessTokenValidator>(options => new JwtTokenValidator(signingKey!));
        }
        private static void AddLoggedUser(IServiceCollection services)
        {
            services.AddScoped<ILoggedUser, LoggedUser>();
        }
        private static void AddPasswordEncrypter(IServiceCollection services, IConfiguration configuration)
        {
            var additionalKey = configuration.GetValue<string>("Settings:Password:AdditionalKey");

            services.AddScoped<IPasswordEncrypter>(option => new Sha512Encrypter(additionalKey!));
        }
        private static void AddOpenAI(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGenerateRecipeAI, ChatGptService>();

            var key = configuration.GetValue<string>("Settings:OpenAI:ApiKey");

            services.AddScoped(c => new ChatClient(MyRecipeBookRuleConstants.CHAT_MODEL, key));
        }
        private static void AddAzureStorage(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("Settings:BlobStorage:Azure");

            if (!string.IsNullOrEmpty(connectionString))
                services.AddScoped<IBlobStorageService>(c => new AzureStorageService(new BlobServiceClient(connectionString)));
        }
        private static void AddQueue(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("Settings:ServiceBus:DeleteUserAccount")!;

            var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            var deleteQueue = new DeleteUserQueue(client.CreateSender("userprofile"));

            var deleteUserProcessor = new DeleteUserProcessor(client.CreateProcessor("userprofile", new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1
            }));

            services.AddSingleton(deleteUserProcessor);

            services.AddScoped<IDeleteUserQueue>(options => deleteQueue);
        }
    }
}
