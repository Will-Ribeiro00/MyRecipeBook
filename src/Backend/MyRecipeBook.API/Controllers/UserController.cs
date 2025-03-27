using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.API.Controllers
{
    public class UserController : MyRecipeBookBaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register(RequestRegisterUserJson request)
        {
            return Created();
        }
    }
}
