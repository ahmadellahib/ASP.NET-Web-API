using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Extensions.Logging;

public static partial class LoggingUser
{
    /// <summary>
    /// Extension method to log whole user properties when created
    /// </summary>
    [LoggerMessage(LogLevel.Information, "User {Action}")]
    public static partial void LogUser(this ILogger logger,string action, [LogProperties] User user);
}