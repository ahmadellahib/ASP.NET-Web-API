namespace CourseLibrary.API.Models;

public interface IConcurrencyAware
{
    string ConcurrencyStamp { get; set; }
}
