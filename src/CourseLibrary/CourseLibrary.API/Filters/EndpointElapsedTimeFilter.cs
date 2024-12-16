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
        _logger.LogInformation("Elapsed time for '{RequestMethod}' '{RequestPath}' response: {ElapsedTime} ms",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);
    }
}