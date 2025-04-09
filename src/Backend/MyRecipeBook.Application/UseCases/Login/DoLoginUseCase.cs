using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly PasswordEncrypter _passwordEncrypter;

        public DoLoginUseCase(IUserReadOnlyRepository repository, PasswordEncrypter passwordEncrypter)
        {
            _repository = repository;
            _passwordEncrypter = passwordEncrypter;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            var encrypterPassword = _passwordEncrypter.Encrypt(request.Password);

            var user = await _repository.GetUserByEmailAndPassword(request.Email, encrypterPassword) ?? throw new InvalidLoginException();

            return  new ResponseRegisteredUserJson
            {
                Name = user.Name
            };
        }
    }
}
