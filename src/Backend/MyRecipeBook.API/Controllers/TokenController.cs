using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Token.RefreshToken;
using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Communication.Responses.Tokens;

namespace MyRecipeBook.API.Controllers
{
    public class TokenController : MyRecipeBookBaseController
    {
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ResponseTokensJson), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromServices] IUseRefreshTokenUseCase useCase,
                                                      [FromBody] RequestNewTokenJson json)
        {
            var response = await useCase.Execute(json);

            return Ok(response);
        }
    }
}
