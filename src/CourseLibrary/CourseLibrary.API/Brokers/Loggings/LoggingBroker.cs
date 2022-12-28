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

    public void LogInformation(string message) =>
        _logger.LogInformation(message);

    public void LogWarning(string message) =>
        _logger.LogWarning(message);

    public void LogError(Exception exception)
    {
        StringBuilder sb = new($"{exception.Message}{Environment.NewLine}");

        if (exception.InnerException != null)
        {
            sb.AppendLine($" Inner exception message: {exception.InnerException.Message}");

            if (exception.InnerException.InnerException != null)
            {
                sb.AppendLine(exception.InnerException.InnerException.Message);
            }

            foreach (object? key in exception.InnerException.Data.Keys)
            {
                string keyMessage = $"   {key}: ";
                sb.AppendLine(keyMessage);

                List<string>? list = exception.InnerException.Data[key] as List<string>;
                if (list is not null)

                    for (int i = 0; i < list.Count; i++)
                    {
                        string? value = list[i];
                        sb.AppendLine($"        {value}");
                    }
            }
        }

        _logger.LogError(sb.ToString(), exception);
    }

    public void LogCritical(Exception exception)
    {
        StringBuilder sb = new($"{exception.Message}");

        if (exception.InnerException != null)
        {
            sb.Append($" Inner exception message: {exception.InnerException.Message}");

            if (exception.InnerException.InnerException != null)
            {
                sb.Append(exception.InnerException.InnerException.Message);
            }
        }

        _logger.LogCritical(exception, sb.ToString());
    }
}
