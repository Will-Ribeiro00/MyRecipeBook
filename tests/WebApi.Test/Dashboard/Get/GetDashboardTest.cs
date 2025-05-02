using CommonTestUtilities.Tokens;
using Shouldly;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.Dashboard.Get
{
    public class GetDashboardTest : MyRecipeBookClassFixture
    {
        private const string METHOD = "dashboard";

        private readonly Guid _userIdentifier;
        public GetDashboardTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Success()
        {
            // Arrange
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
            
            // Act
            var response = await DoGet(method: METHOD, token: token);

            await using var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            responseData.RootElement.GetProperty("recipes").GetArrayLength().ShouldBeGreaterThan(0);
        }
    }
}
