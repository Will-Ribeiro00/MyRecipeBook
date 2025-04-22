using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exception;
using Shouldly;

namespace Validator.Test.Recipe
{
    public class RecipeValidatorTest
    {
        [Fact]
        public void Success()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void SuccessCookingTimeNull()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = null;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void SuccessDifficultLevelNull()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = null;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void ErrorCookingTimeInvalid()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = (CookingTime)1000;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED));
        }

        [Fact]
        public void ErrorDifficultyLevelInvalid()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = (Difficulty)1000;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DIFFICULTY_LEVEL_NOT_SUPPORTED));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ErrorTitleEmpty(string title)
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = title;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY));
        }
    }
}
