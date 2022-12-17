namespace CourseLibrary.API.Models.Exceptions;

public class InvalidEntityException<T> : Exception
{
    public InvalidEntityException() : base($"Invalid entity of type {typeof(T).Name}.")
    { }
}