using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Communication.Responses.Tokens;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Secutiry.Tokens;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Token.RefreshToken
{
    public class UseRefreshTokenUseCase : IUseRefreshTokenUseCase
    {
        private readonly IAccessTokenGenerator _tokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly ITokenRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UseRefreshTokenUseCase(IAccessTokenGenerator tokenGenerator, IRefreshTokenGenerator refreshTokenGenerator, ITokenRepository repository, IUnitOfWork unitOfWork)
        {
            _tokenGenerator = tokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseTokensJson> Execute(RequestNewTokenJson json)
        {
            var refreshToken = await _repository.Get(json.RefreshToken) ?? throw new RefreshTokenNotFoundException();

            var refreshTokenValidUntil = refreshToken.CreatedOn.AddDays(MyRecipeBookRuleConstants.REFRESH_TOKEN_EXPIRATION_DAYS);
            if (DateTime.Compare(refreshTokenValidUntil, DateTime.UtcNow) < 0)
                throw new RefreshTokenExpiredException();

            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                Value = _refreshTokenGenerator.Generate(),
                UserId = refreshToken.UserId
            };

            await _repository.SaveNewRefreshToken(newRefreshToken);

            await _unitOfWork.Commit();

            return new ResponseTokensJson
            {
                AccessToken = _tokenGenerator.Generate(refreshToken.User.UserIdentifier),
                RefreshToken = newRefreshToken.Value
            };
        }
    }
}
