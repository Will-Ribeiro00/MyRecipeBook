using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.FilterAll;
using MyRecipeBook.Exception;
using Shouldly;

namespace Validator.Test.Recipe.Filter
{
    public class FilterRecipeValidatorTest
    {
        [Fact]
        public void Success()
        {
            // Arrange
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void ErrorInvalidCookingTime()
        {
            // Arrange
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED));
        }

        [Fact]
        public void ErrorInvalidDifficulty()
        {
            // Arrange
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.Difficulties.Add((MyRecipeBook.Communication.Enums.Difficulty)1000);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DIFFICULTY_LEVEL_NOT_SUPPORTED));
        }

        [Fact]
        public void ErrorInvalidDishType()
        {
            // Arrange
            var validator = new FilterRecipeValidator();
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.DishTypes.Add((MyRecipeBook.Communication.Enums.DishType)1000);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DISH_TYPE_NOT_SUPPORTED));
        }
    }
}
