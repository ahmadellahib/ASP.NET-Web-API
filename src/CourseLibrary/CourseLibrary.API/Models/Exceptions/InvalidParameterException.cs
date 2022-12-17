namespace CourseLibrary.API.Models.Exceptions;

public class InvalidParameterException : Exception
{
    public string PropertyName { get; set; } = string.Empty;

    public InvalidParameterException(string parameterName) : base($"Passed {parameterName} is invalid")
    {
        PropertyName = parameterName;
    }
}