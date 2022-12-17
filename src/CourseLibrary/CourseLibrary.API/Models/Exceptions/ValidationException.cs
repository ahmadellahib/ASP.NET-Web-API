namespace CourseLibrary.API.Models.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException) { }
}