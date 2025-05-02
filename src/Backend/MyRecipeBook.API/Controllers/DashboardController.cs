﻿using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Attributes;
using MyRecipeBook.Application.UseCases.Dashboard.Get;
using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.API.Controllers
{
    [AuthenticatedUser]
    public class DashboardController : MyRecipeBookBaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResponseRecipesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Get([FromServices] IGetDashboardUseCase useCase)
        {
            var response = await useCase.Execute();

            if (response.Recipes.Any())
                return Ok(response);

            return NoContent();
        }
    }
}
