using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public interface IRegisterUserUseCase
    {
        public Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request);
    }
}
