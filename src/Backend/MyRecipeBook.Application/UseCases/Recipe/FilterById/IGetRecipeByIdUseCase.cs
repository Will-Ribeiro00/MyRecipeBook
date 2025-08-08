using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.FilterById
{
    public interface IGetRecipeByIdUseCase
    {
        public Task<ResponseRecipeJson> Execute(long recipeId);
    }
}
