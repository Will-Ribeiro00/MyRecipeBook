using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Login;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.Exception;
using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.API.Controllers
{
    public class LoginController : MyRecipeBookBaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromServices] IDoLoginUseCase useCase,
                                               [FromBody] RequestLoginJson request)
        {
            var response = await useCase.Execute(request);

            return Ok(response);
        }
    }
}
