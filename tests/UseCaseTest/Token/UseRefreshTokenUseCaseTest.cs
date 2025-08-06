using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.Tokens.RefreshToken;
using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Token
{
    public class UseRefreshTokenUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var refreshToken = RefreshTokenBuilder.Build(user);

            var useCase = CreateUseCase(refreshToken);


            // Act
            var result = await useCase.Execute(new RequestNewTokenJson
            {
                RefreshToken = refreshToken.Value
            });

            // Assert
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrEmpty();
            result.RefreshToken.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Error_RefreshToken_Not_Found()
        {
            // Arrange
            var useCase = CreateUseCase();

            // Act
            var act = async () => await useCase.Execute(new RequestNewTokenJson
            {
                RefreshToken = string.Empty
            });
            var response = await act.ShouldThrowAsync<RefreshTokenNotFoundException>();

            // Assert
            response.GetErrorMessages().ShouldHaveSingleItem();
            response.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.INVALID_SESSION);
        }

        [Fact]
        public async Task Error_RefreshToken_Expired()
        {
            // Arrange
            (var user, _) = UserBuilder.Build();
            var refreshToken = RefreshTokenBuilder.Build(user);
            refreshToken.CreatedOn = DateTime.UtcNow.AddDays(-MyRecipeBookRuleConstants.REFRESH_TOKEN_EXPIRATION_DAYS - 1);

            var useCase = CreateUseCase(refreshToken);

            // Act
            var act = async () => await useCase.Execute(new RequestNewTokenJson
            {
                RefreshToken = refreshToken.Value
            });
            var response = await act.ShouldThrowAsync<RefreshTokenExpiredException>();

            // Assert
            response.GetErrorMessages().ShouldHaveSingleItem();
            response.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.EXPIRED_SESSION);
        }

        private static UseRefreshTokenUseCase CreateUseCase(MyRecipeBook.Domain.Entities.RefreshToken? refreshToken = null)
        {
            var unitOfWork = UnitOfWorkBuilder.Build();
            var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
            var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
            var tokenRepository = new TokenRepositoryBuilder().Get(refreshToken).Build();

            return new UseRefreshTokenUseCase(tokenRepository, refreshTokenGenerator, accessTokenGenerator, unitOfWork);
        }
    }
}
