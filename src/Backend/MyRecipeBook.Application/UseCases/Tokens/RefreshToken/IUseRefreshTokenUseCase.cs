using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Communication.Responses.Tokens;

namespace MyRecipeBook.Application.UseCases.Tokens.RefreshToken
{
    public interface IUseRefreshTokenUseCase
    {
        Task<ResponseTokensJson> Execute(RequestNewTokenJson json);
    }
}
