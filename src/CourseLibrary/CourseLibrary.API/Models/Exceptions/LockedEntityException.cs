namespace CourseLibrary.API.Models.Exceptions;

public class LockedEntityException<T> : Exception
{
    public LockedEntityException(Exception innerException)
        : base($"Locked {typeof(T).Name} record exception, please try again later.", innerException) { }
}