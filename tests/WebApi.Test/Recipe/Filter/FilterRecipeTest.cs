using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Filter
{
    public class FilterRecipeTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe/filter";

        private readonly Guid _userIdentifier;

        private string _recipeTitle;
        private CookingTime _recipeCookingTime;
        private Difficulty _recipeDifficultyLevel;
        private IList<DishType> _recipeDishTypes;
        public FilterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();

            _recipeTitle = factory.GetRecipeTitle();
            _recipeCookingTime = factory.GetRecipeCookingTime();
            _recipeDifficultyLevel = factory.GetRecipeDifficulty();
            _recipeDishTypes = factory.GetRecipeDishTypes();
        }

        [Fact]
        public async Task Success()
        {
            // Arrange and Act
            var request = new RequestFilterRecipeJson
            {
                CookingTimes = [(MyRecipeBook.Communication.Enums.CookingTime)_recipeCookingTime],
                Difficulties = [(MyRecipeBook.Communication.Enums.Difficulty)_recipeDifficultyLevel],
                DishTypes = [.. _recipeDishTypes.Select(dishType => (MyRecipeBook.Communication.Enums.DishType)dishType)],
                RecipeTitle_Ingredient = _recipeTitle
            };

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPost(method: METHOD, request: request, token: token);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            // Assert
            responseData.RootElement.GetProperty("recipes").EnumerateArray().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task SuccessNoContent()
        {
            // Arrange and Act
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.RecipeTitle_Ingredient = "recipeDontExist";

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPost(method: METHOD, request: request, token: token);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorCookingTimeInvalid(string culture)
        {
            // Arrange and Act
            var request = RequestFilterRecipeJsonBuilder.Build();
            request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)100);

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPost(method: METHOD, request: request, token: token, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("COOKING_TIME_NOT_SUPPORTED", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
