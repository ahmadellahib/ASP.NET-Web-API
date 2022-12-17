using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Enums;

namespace CourseLibrary.API.Models.Authors;

public class Author : IConcurrencyAware, IAuditable
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset? DateOfDeath { get; set; }
    public string MainCategory { get; set; } = string.Empty;

    public List<Course> Courses { get; set; } = new();

    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public Guid UpdatedById { get; set; }

    public string ConcurrencyStamp { get; set; } = string.Empty;
}