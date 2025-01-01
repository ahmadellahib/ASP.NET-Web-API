using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Brokers.Logging;

public partial interface ILoggingBroker<T> where T : class
{
    void LogUser(string action, User user);
}
