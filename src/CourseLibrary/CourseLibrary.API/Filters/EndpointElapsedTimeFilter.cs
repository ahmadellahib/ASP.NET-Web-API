using CourseLibrary.API.Brokers.Loggings;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace CourseLibrary.API.Filters;

public class EndpointElapsedTimeFilter : IAsyncActionFilter
{
    private Stopwatch? stopWatch;
    private readonly ILoggingBroker<EndpointElapsedTimeFilter> _logger;

    public EndpointElapsedTimeFilter(ILoggingBroker<EndpointElapsedTimeFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Do something before the action executes.
        stopWatch = new();
        stopWatch.Start();

        await next();

        // Do something after the action executes.
        stopWatch!.Stop();
        TimeSpan ts = stopWatch.Elapsed;

        string _elapsedTimeMessage = String.Format("Elapsed time for '{0}' response: {1:00}:{2:00}:{3:00}.{4:00}",
                               context.HttpContext.Request.Path,
                               ts.Hours,
                               ts.Minutes,
                               ts.Seconds,
                               ts.Milliseconds / 10);

        _logger.LogInformation(_elapsedTimeMessage);
    }
}