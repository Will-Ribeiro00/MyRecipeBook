﻿using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate
{
    public class GenerateRecipeUseCase : IGenerateRecipeUseCase
    {
        private readonly IGenerateRecipeAI _generator;

        public GenerateRecipeUseCase(IGenerateRecipeAI generator)
        {
            _generator = generator;
        }

        public async Task<ResponseGeneratedRecipeJson> Execute(RequestGenerateRecipeJson request)
        {
            Validate(request);

            var response = await _generator.Generate(request.Ingredients);

            return new ResponseGeneratedRecipeJson
            {
                Title = response.Title,
                Ingredients = response.Ingredients,
                CookingTime = (Communication.Enums.CookingTime)response.CookingTime,
                Instructions = response.Instructions.Select(c => new ResponseGeneratedInstructionJson
                {
                    Step = c.Step,
                    Description = c.Description,
                }).ToList(),
                Difficulty = Communication.Enums.Difficulty.Low
            };
        }

        private static void Validate(RequestGenerateRecipeJson request)
        {
            var result = new GenerateRecipeValidator().Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
        }
    }
}
