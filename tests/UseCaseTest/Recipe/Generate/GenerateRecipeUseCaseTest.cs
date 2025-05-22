using CommonTestUtilities.Dtos;
using CommonTestUtilities.OpenAI;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Domain.Dto.Generate;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCaseTest.Recipe.Generate
{
    public class GenerateRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            // Arrange
            var dto = GeneratedRecipeDtoBuild.Build();
            var request = RequestGenerateRecipeJsonBuilder.Build();
            var useCase = CreateUseCase(dto);

            // Act
            var result = await useCase.Execute(request);

            // Assert
            result.ShouldNotBeNull();
            result.Title.ShouldBe(dto.Title);
            result.CookingTime.ShouldBe((MyRecipeBook.Communication.Enums.CookingTime)dto.CookingTime);
            result.Difficulty.ShouldBe(MyRecipeBook.Communication.Enums.Difficulty.Low);
        }

        [Fact]
        public async Task ErrorDuplicatedIngredients()
        {
            // Arrange
            var dto = GeneratedRecipeDtoBuild.Build();
            var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE - 1);
            request.Ingredients.Add(request.Ingredients[0]);

            var useCase = CreateUseCase(dto);

            // Act
            var act = async () => await useCase.Execute(request);

            var result = await act.ShouldThrowAsync<ErrorOnValidationException>();

            // Assert
            result.GetErrorMessages().ShouldHaveSingleItem();
            result.GetErrorMessages().ShouldContain(ResourceMessagesExceptions.DUPLICATED_INGREDIENTS_IN_LIST);
        }

        private static GenerateRecipeUseCase CreateUseCase(GeneratedRecipeDto dto)
        {
            var generateRecipeAI = GenerateRecipeAIBuilder.Build(dto);

            return new GenerateRecipeUseCase(generateRecipeAI);
        }
    }
}
