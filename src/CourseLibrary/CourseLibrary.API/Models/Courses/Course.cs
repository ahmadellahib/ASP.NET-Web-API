using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Users;
using System.Diagnostics;

namespace CourseLibrary.API.Models.Courses;

[DebuggerDisplay("{Title,nq}")]
public class Course : IConcurrencyAware, IAuditable
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public Guid UpdatedById { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public Author Author { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public User UpdatedBy { get; set; } = null!;
}