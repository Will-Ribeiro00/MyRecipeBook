using CommonTestUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.GetById
{
    public class GetRecipeByIdTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe";

        private readonly Guid _userIdentifier;
        private readonly string _recipeId;
        private readonly string _recipeTitle;

        public GetRecipeByIdTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _recipeId = factory.GetRecipeId();
            _recipeTitle = factory.GetRecipeTitle();
        }

        [Fact]
        public async Task Success()
        {
            // Arrange
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // Act
            var response = await DoGet(method: $"{METHOD}/{_recipeId}", token: token);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            responseData.RootElement.GetProperty("id").GetString().ShouldBe(_recipeId);
            responseData.RootElement.GetProperty("title").GetString().ShouldBe(_recipeTitle);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorRecipeNotFound(string culture)
        {
            // Arrange
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
            var id = IdEncrypterBuilder.Build().Encode(1000);

            // Act
            var response = await DoGet(method: $"{METHOD}/{id}", token: token, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var erros = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            erros.ShouldHaveSingleItem();
            erros.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
