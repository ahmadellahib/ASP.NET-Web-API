namespace CourseLibrary.API.Brokers.Logging;

internal sealed class LoggingBroker<T> : ILoggingBroker<T> where T : class
{
    private readonly ILogger<T> _logger;

    public LoggingBroker(ILogger<T> logger) =>
        _logger = logger;

    public bool IsEnabled(LogLevel logLevel) =>
        _logger.IsEnabled(logLevel);

    public void LogDebug(string message) =>
        _logger.LogDebug(message);

    public void LogTrace(string message) =>
        _logger.LogTrace(message);

    public void LogInformation(string message, params object?[] args) =>
        _logger.LogInformation(message, args);

    public void LogWarning(string message, params object?[] args) =>
        _logger.LogWarning(message, args);

    public void LogError(string scheme, string requestMethod, string requestPath, Exception exception)
    {
        string message = exception.Message;
        string innerExceptionMessage = exception.InnerException?.Message ?? "No inner exception";

        string msgTemplate = scheme + " {RequestMethod} {RequestPath} {Message} Inner Exception: {InnerException}";
        _logger.LogError(exception, msgTemplate, requestMethod, requestPath, message, innerExceptionMessage);
    }

    public void LogCritical(string scheme, string requestMethod, string requestPath, Exception exception)
    {
        string message = exception.Message;
        string innerExceptionMessage = exception.InnerException?.Message ?? "No inner exception";

        string msgTemplate = scheme + " {RequestMethod} {RequestPath} {Message} Inner Exception: {InnerException}";
        _logger.LogCritical(exception, msgTemplate, requestMethod, requestPath, message, innerExceptionMessage);
    }
}
