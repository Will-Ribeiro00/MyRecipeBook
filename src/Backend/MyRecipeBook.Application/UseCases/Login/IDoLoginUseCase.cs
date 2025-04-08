using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.Application.UseCases.Login
{
    public interface IDoLoginUseCase
    {
        public Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request);
    }
}
