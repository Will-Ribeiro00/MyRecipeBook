using CommonTestUtilities.Requests;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Register
{
    public class RegisterUserTest : MyRecipeBookClassFixture
    {
        private readonly string _method = "user";

        public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Success()
        {
            // Arrange
            var request = RequestRegisterUserJsonBuilder.Build();
            var response = await DoPost(_method, request);

            // Act
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            responseData.RootElement.GetProperty("name").GetString().ShouldNotBeNull();
            responseData.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        }
        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorNameEmpty(string culture)
        {
            // Arrange
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = string.Empty;

            var response = await DoPost(_method, request, culture);

            // Act
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
