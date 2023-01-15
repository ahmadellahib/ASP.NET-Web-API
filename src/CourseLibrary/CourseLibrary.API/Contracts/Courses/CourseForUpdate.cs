using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.Contracts.Courses;

public class CourseForUpdate
{
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UpdatedById { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator Course(CourseForUpdate courseForUpdate) => new()
    {
        AuthorId = courseForUpdate.AuthorId,
        Title = courseForUpdate.Title,
        Description = courseForUpdate.Description,
        UpdatedById = courseForUpdate.UpdatedById,
        ConcurrencyStamp = courseForUpdate.ConcurrencyStamp
    };
}