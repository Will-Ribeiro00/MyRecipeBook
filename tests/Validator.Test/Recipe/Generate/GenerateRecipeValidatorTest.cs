using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exception;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace Validator.Test.Recipe.Generate
{
    public class GenerateRecipeValidatorTest
    {
        [Fact]
        public void Success()
        {
            // Arrange
            var validator = new GenerateRecipeValidator();
            var request = RequestGenerateRecipeJsonBuilder.Build();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void ErrorMoreMaximumIngredient()
        {
            // Arrange
            var validator = new GenerateRecipeValidator();
            var request = RequestGenerateRecipeJsonBuilder.Build(count: MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE + 1);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INVALID_NUMBER_INGREDIENTS));
        }

        [Fact]
        public void ErrorDuplicatedIngredient()
        {
            // Arrange
            var validator = new GenerateRecipeValidator();
            var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE - 1);
            request.Ingredients.Add(request.Ingredients[0]);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DUPLICATED_INGREDIENTS_IN_LIST));
        }

        [Fact]
        public void ErrorIngredientNotFollowingPatter()
        {
            // Arrange
            var validator = new GenerateRecipeValidator();
            var request = RequestGenerateRecipeJsonBuilder.Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE - 1);
            request.Ingredients.Add("This is an invalid ingredient because is too long");

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INGREDIENT_NOT_FOLLOWING_PATTERN));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ErrorEmptyIngredient(string ingredient)
        {
            // Arrange
            var validator = new GenerateRecipeValidator();
            var request = RequestGenerateRecipeJsonBuilder.Build(count: 1);
            request.Ingredients.Add(ingredient);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INGREDIENT_EMPTY));
        }
    }
}
