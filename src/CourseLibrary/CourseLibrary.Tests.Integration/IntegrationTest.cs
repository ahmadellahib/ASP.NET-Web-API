using Microsoft.AspNetCore.Mvc.Testing;
using Tynamix.ObjectFiller;

namespace CourseLibrary.Tests.Integration;

public class IntegrationTest
{
    protected HttpClient _httpClient { get; set; }

    protected IntegrationTest()
    {
        WebApplicationFactory<Program> appFactory = new();
        _httpClient = appFactory.CreateClient();
    }

    protected static DateTimeOffset GetRandomDateTime() =>
        new DateTimeRange(earliestDate: new DateTime()).GetValue();
}