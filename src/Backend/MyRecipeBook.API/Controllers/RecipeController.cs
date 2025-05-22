using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Attributes;
using MyRecipeBook.API.Binders;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Application.UseCases.Recipe.FilterAll;
using MyRecipeBook.Application.UseCases.Recipe.FilterById;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Exception;
using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.API.Controllers
{
    [AuthenticatedUser]
    public class RecipeController : MyRecipeBookBaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredRecipeJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromServices] IRegisterRecipeUseCase useCase,
                                                  [FromBody] RequestRecipeJson request)
        {
            var response = await useCase.Execute(request);

            return Created(string.Empty, response);
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(ResponseRecipesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Filter([FromServices] IFilterRecipeUseCase useCase,
                                                [FromBody] RequestFilterRecipeJson request)
        {
            var response = await useCase.Execute(request);

            if (response.Recipes.Any())
                return Ok(response);

            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseRecipeJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromServices] IGetRecipeByIdUseCase useCase,
                                                 [FromRoute] [ModelBinder(typeof(MyRecipeBookIdBinder))]long id)
        {
            var response = await useCase.Execute(id);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromServices] IDeleteRecipeUseCase useCase,
                                                [FromRoute][ModelBinder(typeof(MyRecipeBookIdBinder))] long id)
        {
            await useCase.Execute(id);

            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromServices] IUpdateRecipeUseCase useCase,
                                                [FromBody] RequestRecipeJson request,
                                                [FromRoute][ModelBinder(typeof(MyRecipeBookIdBinder))] long id)
        {
            await useCase.Execute(id, request);

            return NoContent();
        }

        [HttpPost("generate")]
        [ProducesResponseType(typeof(ResponseGeneratedRecipeJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Generate([FromServices] IGenerateRecipeUseCase useCase,
                                                  [FromBody] RequestGenerateRecipeJson request)
        {
            var response = await useCase.Execute(request);

            return Ok(response);
        }
    }
}
