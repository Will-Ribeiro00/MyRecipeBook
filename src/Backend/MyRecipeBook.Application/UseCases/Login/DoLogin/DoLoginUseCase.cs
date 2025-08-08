using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.Tokens;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Secutiry.Cryptography;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly ITokenRepository _tokenRepository;

        public DoLoginUseCase(IUserReadOnlyRepository repository, IPasswordEncrypter passwordEncrypter, IAccessTokenGenerator accessTokenGenerator, IUnitOfWork unitOfWork, IRefreshTokenGenerator refreshTokenGenerator, ITokenRepository tokenRepository)
        {
            _repository = repository;
            _passwordEncrypter = passwordEncrypter;
            _accessTokenGenerator = accessTokenGenerator;
            _unitOfWork = unitOfWork;
            _refreshTokenGenerator = refreshTokenGenerator;
            _tokenRepository = tokenRepository;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            var user = await _repository.GetByEmail(request.Email);

            if (user is null || !_passwordEncrypter.IsValid(request.Password, user.Password))
                throw new InvalidLoginException();

            var refreshToken = await CreateAndSaveRefreshToken(user);

            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier),
                    RefreshToken = refreshToken
                }
            };
        }

        private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
        {
            var refreshToken = _refreshTokenGenerator.Generate();

            await _tokenRepository.SaveNewRefreshToken(new RefreshToken
            {
                Value = refreshToken,
                UserId = user.Id
            });

            await _unitOfWork.Commit();

            return refreshToken;
        }
    }
}
