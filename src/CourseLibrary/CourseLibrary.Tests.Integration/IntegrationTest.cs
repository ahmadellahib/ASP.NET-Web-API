using Microsoft.AspNetCore.Mvc.Testing;

namespace CourseLibrary.Tests.Integration;

public class IntegrationTest
{
    protected HttpClient _httpClient { get; set; }

    protected IntegrationTest()
    {
        WebApplicationFactory<Program> appFactory = new();
        _httpClient = appFactory.CreateClient();
    }
}