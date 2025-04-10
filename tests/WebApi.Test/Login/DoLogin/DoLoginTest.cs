using CommonTestUtilities.Requests;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Login.DoLogin
{
    public class DoLoginTest : MyRecipeBookClassFixture
    {
        private readonly string _method = "login";

        private readonly string _email; 
        private readonly string _password; 
        private readonly string _name; 
        public DoLoginTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _email = factory.GetEmail();
            _password = factory.GetPassword();
            _name = factory.GetName();
        }

        [Fact]
        public async Task Success()
        {
            // Arrange
            var request = new RequestLoginJson
            {
                Email = _email,
                Password = _password
            };

            // Act
            var response = await DoPost(_method, request);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            responseData.RootElement.GetProperty("name").GetString().ShouldNotBeNullOrWhiteSpace();
            responseData.RootElement.GetProperty("name").GetString().ShouldBe(_name);
            responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeNullOrEmpty();
        }
        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorLoginInvalid(string culture)
        {
            // Arrange
            var request = RequestLoginJsonBuilder.Build();

            var response = await DoPost(_method, request, culture);

            // Act
            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
