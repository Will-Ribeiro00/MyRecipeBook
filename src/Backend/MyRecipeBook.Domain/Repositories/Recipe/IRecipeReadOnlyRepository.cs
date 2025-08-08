using MyRecipeBook.Domain.Dto.Recipe;

namespace MyRecipeBook.Domain.Repositories.Recipe
{
    public interface IRecipeReadOnlyRepository
    {
        Task<IList<Entities.Recipe>> Filter(Entities.User user, FilterRecipesDto filters);
        Task<Entities.Recipe?> GetById(Entities.User user, long recipeId);
        Task<bool> RecipeExist(Entities.User user, long recipeId);
        Task<IList<Entities.Recipe>> GetForDashboard(Entities.User user);
    }
}
