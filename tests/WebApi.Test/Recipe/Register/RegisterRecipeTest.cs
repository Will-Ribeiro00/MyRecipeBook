using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Register
{
    public class RegisterRecipeTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "recipe";

        private readonly Guid _userIdentifier;
        public RegisterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Success()
        {
            // Arrange
            var request = RequestRecipeJsonBuilder.Build();
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // Act
            var response = await DoPost(method: METHOD, request: request, token: token);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            responseData.RootElement.GetProperty("title").GetString().ShouldBe(request.Title);
            responseData.RootElement.GetProperty("id").GetString().ShouldNotBeNullOrWhiteSpace();
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorTitleEmpty(string culture)
        {
            // Arrange
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // Act
            var response = await DoPost(method: METHOD, request: request, token: token, culture: culture);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
