using CommonTestUtilities.Entities;
using CommonTestUtilities.IdEncryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Infrastructure.DataAccess;

namespace WebApi.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private MyRecipeBook.Domain.Entities.User _user = default!;
        private MyRecipeBook.Domain.Entities.Recipe _recipe = default!;
        private string _password = string.Empty;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test")
                .ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MyRecipeBookDbContext>));
                    if (descriptor is not null)
                        services.Remove(descriptor);

                    var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
                
                    services.AddDbContext<MyRecipeBookDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(provider);
                    });

                    using var scope = services.BuildServiceProvider().CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();

                    context.Database.EnsureDeleted();
                    StartDatabase(context);
                });
        }


        // User Infos
        public string GetEmail() => _user.Email;
        public string GetPassword() => _password;
        public string GetName() => _user.Name;
        public Guid GetUserIdentifier() => _user.UserIdentifier;

        //Recipe Infos
        public string GetRecipeId() => IdEncrypterBuilder.Build().Encode(_recipe.Id);
        public string GetRecipeTitle() => _recipe.Title;
        public CookingTime GetRecipeCookingTime() => _recipe.CookingTime!.Value;
        public Difficulty GetRecipeDifficulty() => _recipe.Difficulty!.Value;
        public IList<DishType> GetRecipeDishTypes() => [.. _recipe.DishTypes.Select(c => c.Type)];

        private void StartDatabase(MyRecipeBookDbContext context)
        {
            (_user, _password) = UserBuilder.Build();
            _recipe = RecipeBuilder.Build(_user);

            context.Users.Add(_user);
            context.Recipes.Add(_recipe);

            context.SaveChanges();
        }
    }
}
