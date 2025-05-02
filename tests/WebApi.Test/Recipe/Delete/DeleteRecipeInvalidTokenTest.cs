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
    public class DeleteRecipeInvalidTokenTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe";
        public DeleteRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorTokenInvalid(string culture)
        {
            // Arrange and Act
            var recipeId = IdEncrypterBuilder.Build().Encode(1);
            var response = await DoDelete(method: $"{METHOD}/{recipeId}", token: "tokenInvalido", culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("USER_WITHOUT_PERMISSION_ACCESS_RESOURCE", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorWithoutToken(string culture)
        {
            // Arrange and Act
            var recipeId = IdEncrypterBuilder.Build().Encode(1);
            var response = await DoDelete(method: $"{METHOD}/{recipeId}", token: string.Empty, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("WITHOUT_TOKEN", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorTokenWithUserNotFound(string culture)
        {
            // Arrange
            var recipeId = IdEncrypterBuilder.Build().Encode(1);
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            // Act
            var response = await DoDelete(method: $"{METHOD}/{recipeId}", token: token, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("USER_WITHOUT_PERMISSION_ACCESS_RESOURCE", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
