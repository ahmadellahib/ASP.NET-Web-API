namespace CourseLibrary.API.Models.Exceptions;

public class DbConflictException : Exception
{
    public DbConflictException(Exception exception) : base(exception.Message)
    {
    }
}