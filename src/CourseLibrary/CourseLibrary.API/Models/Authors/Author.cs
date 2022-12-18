using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Models.Authors;

public class Author : IConcurrencyAware
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string MainCategory { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public virtual User User { get; set; } = null!;
    public virtual IEnumerable<Course>? Courses { get; set; }
}