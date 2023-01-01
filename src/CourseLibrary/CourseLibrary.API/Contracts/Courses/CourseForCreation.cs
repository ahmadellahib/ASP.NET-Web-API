using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.Contracts.Courses;

public class CourseForCreation
{
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedById { get; set; }

    public static explicit operator Course(CourseForCreation courseForCreation) => new()
    {
        AuthorId = courseForCreation.AuthorId,
        Title = courseForCreation.Title,
        Description = courseForCreation.Description,
        CreatedById = courseForCreation.CreatedById
    };
}