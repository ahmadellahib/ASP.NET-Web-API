using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Models.Courses;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AuthorId { get; set; }

    public Author Author { get; set; } = null!;
}