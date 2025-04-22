﻿using FluentValidation;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Exception;

namespace MyRecipeBook.Application.UseCases.Recipe
{
    public class RecipeValidator : AbstractValidator<RequestRecipeJson>
    {
        public RecipeValidator()
        {
            RuleFor(recipe => recipe.Title).NotEmpty()
                .WithMessage(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY);

            RuleFor(recipe => recipe.CookingTime).IsInEnum()
                .WithMessage(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED);

            RuleFor(recipe => recipe.Difficulty).IsInEnum()
                .WithMessage(ResourceMessagesExceptions.DIFFICULTY_LEVEL_NOT_SUPPORTED);
        }
    }
}
