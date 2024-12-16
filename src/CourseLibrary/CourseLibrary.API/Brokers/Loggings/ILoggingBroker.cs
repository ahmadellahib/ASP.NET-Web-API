namespace CourseLibrary.API.Brokers.Loggings;

public interface ILoggingBroker<T> where T : class
{
    bool IsEnabled(LogLevel logLevel);

    void LogTrace(string message);

    void LogDebug(string message);

    void LogInformation(string message, params object?[] args);

    void LogWarning(string message, params object?[] args);

    void LogError(Exception exception, string instance);

    void LogCritical(string instance, Exception exception);
}