using CourseLibrary.API.Extensions.Logging;
using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Brokers.Logging;

internal sealed partial class LoggingBroker<T> where T : class
{
    /// <summary>
    /// Log User properties when created
    /// </summary>
    public void LogUser(string action, User user)
    {
        _logger.LogUser(action,user);
    }
}
