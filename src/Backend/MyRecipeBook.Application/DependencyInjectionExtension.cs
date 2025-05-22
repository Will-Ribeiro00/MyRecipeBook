using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.Services.AutoMapper;
using MyRecipeBook.Application.UseCases.Dashboard.Get;
using MyRecipeBook.Application.UseCases.Login;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Application.UseCases.Recipe.FilterAll;
using MyRecipeBook.Application.UseCases.Recipe.FilterById;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
using Sqids;

namespace MyRecipeBook.Application
{
    public static class DependencyInjectionExtension
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddAutoMapper(services);
            AddIdEncoder(services, configuration);
            AddUseCases(services);
        }

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
            services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();

            services.AddScoped<IRegisterRecipeUseCase, RegisterRecipeUseCase>();
            services.AddScoped<IFilterRecipeUseCase, FilterRecipeUseCase>();
            services.AddScoped<IGetRecipeByIdUseCase, GetRecipeByIdUseCase>();
            services.AddScoped<IDeleteRecipeUseCase, DeleteRecipeUseCase>();
            services.AddScoped<IUpdateRecipeUseCase, UpdateRecipeUseCase>();

            services.AddScoped<IGetDashboardUseCase, GetDashboardUseCase>();
            services.AddScoped<IGenerateRecipeUseCase, GenerateRecipeUseCase>();
        }
        private static void AddIdEncoder(IServiceCollection services, IConfiguration configuration)
        {
            var sqids = new SqidsEncoder<long>(new()
            {
                MinLength = 3,
                Alphabet = configuration.GetValue<string>("Settings:IdCryptographyAlphabet")!
            });

            services.AddSingleton(sqids);
        }
        private static void AddAutoMapper(IServiceCollection services)
        {

            services.AddScoped(option => new AutoMapper.MapperConfiguration(AutoMapperOption =>
                {
                    var sqids = option.GetService<SqidsEncoder<long>>()!;

                    AutoMapperOption.AddProfile(new AutoMapping(sqids));
                }).CreateMapper()
            );
        }
    }
}
