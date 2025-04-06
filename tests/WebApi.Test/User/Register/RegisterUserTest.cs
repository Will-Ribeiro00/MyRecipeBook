using CommonTestUtilities.Requests;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Register
{
    public class RegisterUserTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        public RegisterUserTest(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();

        [Fact]
        public async Task Success()
        {
            // Arrange
            var request = RequestRegisterUserJsonBuilder.Build();
            var response = await _httpClient.PostAsJsonAsync("User", request);
            
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

            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);

            var response = await _httpClient.PostAsJsonAsync("User", request);

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
