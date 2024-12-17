namespace CourseLibrary.API.Brokers.Logging;

public interface ILoggingBroker<T> where T : class
{
    bool IsEnabled(LogLevel logLevel);

    void LogTrace(string message);

    void LogDebug(string message);

    void LogInformation(string message, params object?[] args);

    void LogWarning(string message, params object?[] args);

    void LogError(string scheme, string requestMethod, string requestPath, Exception exception);

    void LogCritical(string scheme, string requestMethod, string requestPath, Exception exception);
}