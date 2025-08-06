using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Communication.Responses.Tokens;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Tokens.RefreshToken
{
    public class UseRefreshTokenUseCase : IUseRefreshTokenUseCase
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public UseRefreshTokenUseCase(ITokenRepository tokenRepository, IRefreshTokenGenerator refreshTokenGenerator, IAccessTokenGenerator accessTokenGenerator, IUnitOfWork unitOfWork)
        {
            _tokenRepository = tokenRepository;
            _refreshTokenGenerator = refreshTokenGenerator;
            _accessTokenGenerator = accessTokenGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseTokensJson> Execute(RequestNewTokenJson json)
        {
            var refreshToken = await _tokenRepository.Get(json.RefreshToken) ?? throw new RefreshTokenNotFoundException();

            var refreshTokenValidUntil = refreshToken.CreatedOn.AddDays(MyRecipeBookRuleConstants.REFRESH_TOKEN_EXPIRATION_DAYS);
            if (DateTime.Compare(refreshTokenValidUntil, DateTime.UtcNow) < 0)
                throw new RefreshTokenExpiredException();

            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                Value = _refreshTokenGenerator.Generate(),
                UserId = refreshToken.UserId
            };

            await _tokenRepository.SaveNewRefreshToken(newRefreshToken);

            await _unitOfWork.Commit();

            return new ResponseTokensJson
            {
                AccessToken = _accessTokenGenerator.Generate(refreshToken.User.UserIdentifier),
                RefreshToken = newRefreshToken.Value
            };
        }
    }
}
