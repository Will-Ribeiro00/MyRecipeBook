using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.Tokens;
using MyRecipeBook.Communication.Responses.User;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly ITokenRepository _tokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public DoLoginUseCase(IUserReadOnlyRepository repository, IPasswordEncrypter passwordEncrypter, IUnitOfWork unitOfWork, IAccessTokenGenerator accessTokenGenerator, ITokenRepository tokenRepository, IRefreshTokenGenerator refreshTokenGenerator)
        {
            _repository = repository;
            _passwordEncrypter = passwordEncrypter;
            _unitOfWork = unitOfWork;
            _accessTokenGenerator = accessTokenGenerator;
            _tokenRepository = tokenRepository;
            _refreshTokenGenerator = refreshTokenGenerator;
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
            var refreshToken = new Domain.Entities.RefreshToken
            {
                Value = _refreshTokenGenerator.Generate(),
                UserId = user.Id,
            };

            await _tokenRepository.SaveNewRefreshToken(refreshToken);

            await _unitOfWork.Commit();

            return refreshToken.Value;
        }
    }
}
