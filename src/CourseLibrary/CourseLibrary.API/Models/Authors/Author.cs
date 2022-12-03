using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.Models.Authors;

internal class Author
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset? DateOfDeath { get; set; }
    public string MainCategory { get; set; } = string.Empty;

    public List<Course> Courses { get; set; } = new();
}