using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Models.Courses;

public class Course : IConcurrencyAware, IAuditable
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AuthorId { get; set; }

    public Author Author { get; set; } = null!;

    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public Guid UpdatedById { get; set; }

    public string ConcurrencyStamp { get; set; } = string.Empty;
}