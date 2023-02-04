using FluentAssertions;
using System.Net;

namespace CourseLibrary.Tests.Integration.Controllers.Home;

public class HomeControllerTests : IClassFixture<IntegrationTestFactory>
{
    private const string HomeRelatativeUrl = "api/home";
    private readonly HttpClient _httpClient;

    public HomeControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _httpClient = integrationTestFactory.CreateClient();
    }

    [Fact]
    public async Task ShouldGetHomeMessageAsync()
    {
        // Arrange
        string expectedHomeMessage = "Welcome to Course Library API";

        // Act
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(HomeRelatativeUrl);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        (await responseMessage.Content.ReadAsStringAsync()).Should().Be(expectedHomeMessage);
    }
}