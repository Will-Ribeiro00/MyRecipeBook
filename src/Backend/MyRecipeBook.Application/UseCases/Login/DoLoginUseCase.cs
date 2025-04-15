using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.Tokens;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Secutiry.Cryptography;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        public DoLoginUseCase(IUserReadOnlyRepository repository, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator)
        {
            _repository = repository;
            _passwordEncrypter = passwordEncrypter;
            _accessTokenGenerator = accessTokenGenerator;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            var encrypterPassword = _passwordEncrypter.Encrypt(request.Password);

            var user = await _repository.GetUserByEmailAndPassword(request.Email, encrypterPassword) ?? throw new InvalidLoginException();

            return  new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier)
                }
            };
        }
    }
}
