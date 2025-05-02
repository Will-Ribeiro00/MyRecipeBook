using MyRecipeBook.Communication.Requests.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Update
{
    public interface IUpdateRecipeUseCase
    {
        public Task Execute(long recipeId, RequestRecipeJson request);
    }
}
