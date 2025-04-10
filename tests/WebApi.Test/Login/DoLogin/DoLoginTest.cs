using CommonTestUtilities.Requests;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Login.DoLogin
{
    public class DoLoginTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly string _method = "login";
        private readonly HttpClient _httpClient;

        private readonly string _email; 
        private readonly string _password; 
        private readonly string _name; 
        public DoLoginTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();

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
            var response = await _httpClient.PostAsJsonAsync(_method, request);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            responseData.RootElement.GetProperty("name").GetString().ShouldNotBeNullOrWhiteSpace();
            responseData.RootElement.GetProperty("name").GetString().ShouldBe(_name);
        }
        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorLoginInvalid(string culture)
        {
            // Arrange
            var request = RequestLoginJsonBuilder.Build();

            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);

            var response = await _httpClient.PostAsJsonAsync(_method, request);

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
