using CommonTestUtilities.Tokens;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Profile
{
    public class GetUserProfileInvalidTokenTest : MyRecipeBookClassFixture
    {
        private readonly string _METHOD = "user";

        public GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorTokenInvalid(string culture)
        {
            // Arrange and Act
            var response = await DoGet(_METHOD, token: "tokenInvalido", culture);

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
            var response = await DoGet(_METHOD, token: string.Empty, culture);

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
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            // Act
            var response = await DoGet(_METHOD, token: token, culture);

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
