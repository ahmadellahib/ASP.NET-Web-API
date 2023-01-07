namespace CourseLibrary.API.Models.Exceptions;

public class DependencyException<T> : Exception, IDependencyException where T : class
{
    public DependencyException(Exception innerException)
        : base($"{typeof(T).Name} dependency error occurred, contact support.", innerException) { }

    public DependencyException(Exception innerException, string extraMessage)
        : base($"{typeof(T).Name} dependency error occurred, contact support. {extraMessage}", innerException) { }
}
