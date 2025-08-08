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
        public void SuccessDishTypeEmpty()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.DishTypes.Clear();

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

        [Fact]
        public void ErrorSameStepInstruction()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Step = request.Instructions.Last().Step;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER));
        }

        [Fact]
        public void ErrorNegativeStepInstruction()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Step = -1;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.NON_NEGATIVE_INSTRUCTION_STEP));
        }

        [Fact]
        public void ErrorDishTypeInvalid()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.DishTypes.Add((DishType)1000);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DISH_TYPE_NOT_SUPPORTED));
        }

        [Fact]
        public void ErrorIngredientsEmpty()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredients.Clear();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.AT_LEAST_ONE_INGREDIENT));
        }

        [Fact]
        public void ErrorInstructionsEmpty()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.Clear();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.AT_LEAST_ONE_INSTRUCTION));
        }

        [Fact]
        public void ErrorInstructionsTooLong()
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Description = RequestStringGenerator.Paragraphs(minCharacters: 2001);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS));
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ErrorIngredientValueEmpty(string ingredient)
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredients.Add(ingredient);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INGREDIENT_EMPTY));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ErrorInstructionValueEmpty(string instruction)
        {
            // Arrange
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Description = instruction;

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INSTRUCTION_EMPTY));
        }
    }
}
