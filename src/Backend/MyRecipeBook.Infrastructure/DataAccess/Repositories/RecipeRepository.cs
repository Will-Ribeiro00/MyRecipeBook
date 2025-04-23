using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    public sealed class RecipeRepository : IRecipeWriteOnlyRepository
    {
        private readonly MyRecipeBookDbContext _context;

        public RecipeRepository(MyRecipeBookDbContext context) => _context = context;

        public async Task Add(Recipe recipe) => await _context.Recipes.AddAsync(recipe);
    }
}
