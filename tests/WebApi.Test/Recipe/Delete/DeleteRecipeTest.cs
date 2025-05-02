using CommonTestUtilities.IdEncryption;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Delete
{
    public class DeleteRecipeTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe";

        private readonly Guid _userIdentifier;
        private readonly string _recipeId;
        public DeleteRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _recipeId = factory.GetRecipeId();
        }

        [Fact]
        public async Task Success()
        {
            // Arrange
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // Act and Assert, step 1
            var response = await DoDelete(method: $"{METHOD}/{_recipeId}", token: token);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // Act and Assert, step 2
            response = await DoGet(method: $"{METHOD}/{_recipeId}", token: token);
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorRecipeNotFound(string culture)
        {
            // Arrage
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
            var id = IdEncrypterBuilder.Build().Encode(1000);

            // Act
            var response = await DoDelete(method: $"{METHOD}/{_recipeId}", token: token, culture: culture);
           

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
