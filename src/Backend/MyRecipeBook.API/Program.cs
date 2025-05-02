using Microsoft.OpenApi.Models;
using MyRecipeBook.API.Converters;
using MyRecipeBook.API.Filters;
using MyRecipeBook.API.Middleware.Culture;
using MyRecipeBook.API.Token;
using MyRecipeBook.Application;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Infrastructure;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Migrations;

const string _bearer = "Bearer";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new StringConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<IdsFilter>();

    options.AddSecurityDefinition(_bearer, new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below
                        Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = _bearer
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = _bearer
                },
                Scheme = "oauth2",
                Name = _bearer,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
});

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRouting(option => option.LowercaseUrls = true);

builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

MigrateDatabase();

app.Run();


void MigrateDatabase()
{
    if (builder.Configuration.IsUnitTestEnviroment())
        return;

    var connectionString = builder.Configuration.ConnectionString();

    var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

    DatabaseMigration.Migrate(connectionString, serviceScope.ServiceProvider);
}

public partial class Program 
{
    protected Program() { }
}