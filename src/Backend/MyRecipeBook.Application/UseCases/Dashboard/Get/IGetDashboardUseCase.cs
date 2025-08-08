using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Dashboard.Get
{
    public interface IGetDashboardUseCase
    {
        public Task<ResponseRecipesJson> Execute();
    }
}
