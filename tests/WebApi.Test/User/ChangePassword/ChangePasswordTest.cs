using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.ChangePassword
{
    public class ChangePasswordTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "user/change-password";

        private readonly string _password;
        private readonly string _email;
        private readonly Guid _userIdentifier;
        public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _password = factory.GetPassword();
            _email = factory.GetEmail();
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Success()
        {
            // arrange
            var request = RequestChangePasswordJsonBuilder.Build();
            request.Password = _password;
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            // Act
            var response = await DoPut(METHOD, request, token);

            // Assert 
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var logginRequest = new RequestLoginJson
            {
                Email = _email,
                Password = _password
            };

            // <- FIRST TEST AFTER PASSWORD CHANGE ->
            // Act and Assert
            response = await DoPost("login", logginRequest);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

            // <- SECOND TEST AFTER PASSWORD CHANGE ->
            // Arrange, Act and Assert
            logginRequest.Password = request.NewPassword;
            response = await DoPost("login", logginRequest);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task ErrorNewPasswordEmpty(string culture)
        {
            // arrange
            var request = new RequestChangePasswordJson
            {
                Password = _password,
                NewPassword = string.Empty
            };
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
            var response = await DoPut(METHOD, request, token, culture);

            // Act
            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
            var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("PASSWORD_INVALID", new CultureInfo(culture));

            // Assert 
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            errors.ShouldHaveSingleItem();
            errors.ShouldContain(error => error.GetString()!.Equals(expectedMessage));
        }
    }
}
