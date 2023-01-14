using CourseLibrary.API.Brokers.Loggings;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace CourseLibrary.API.Filters;

internal sealed class EndpointElapsedTimeFilter : IAsyncActionFilter
{
    private readonly ILoggingBroker<EndpointElapsedTimeFilter> _logger;

    public EndpointElapsedTimeFilter(ILoggingBroker<EndpointElapsedTimeFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Do something before the action executes.
        long startTime = Stopwatch.GetTimestamp();

        await next();

        // Do something after the action executes.
        string _elapsedTimeMessage = String.Format("Elapsed time for '{0}' response: {1}ms",
                               context.HttpContext.Request.Path,
                               Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);

        _logger.LogInformation(_elapsedTimeMessage);
    }
}