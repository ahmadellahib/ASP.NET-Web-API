namespace CourseLibrary.API.Brokers.Loggings;

public interface ILoggingBroker<T> where T : class
{
    bool IsEnabled(LogLevel logLevel);

    void LogTrace(string message);

    void LogDebug(string message);

    void LogInformation(string message);

    void LogWarning(string message);

    void LogError(Exception exception);

    void LogCritical(Exception exception);
}