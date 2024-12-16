using System.Text;

namespace CourseLibrary.API.Brokers.Loggings;

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

    public void LogError(Exception exception, string instance)
    {
        StringBuilder sb = new($"{instance} {exception.Message}{Environment.NewLine}");

        if (exception.InnerException != null)
        {
            sb.AppendLine($" Inner exception message: {exception.InnerException.Message}");

            if (exception.InnerException.InnerException != null)
            {
                sb.AppendLine(exception.InnerException.InnerException.Message);
            }
        }

        _logger.LogError(sb.ToString(), exception);
    }

    public void LogCritical(string instance, Exception exception)
    {
        StringBuilder sb = new($"{instance} {exception.Message}{Environment.NewLine}");

        if (exception.InnerException != null)
        {
            sb.Append($" Inner exception message: {exception.InnerException.Message}");

            if (exception.InnerException.InnerException != null)
            {
                sb.Append(exception.InnerException.InnerException.Message);
            }
        }

        sb.Append($"{Environment.NewLine}StackTrace: {exception.StackTrace}");

        _logger.LogCritical(exception, sb.ToString());
    }
}
