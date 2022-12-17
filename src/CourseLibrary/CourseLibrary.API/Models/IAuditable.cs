namespace CourseLibrary.API.Models;

public interface IAuditable
{
    DateTimeOffset CreatedDate { get; set; }
    DateTimeOffset UpdatedDate { get; set; }
    Guid CreatedById { get; set; }
    Guid UpdatedById { get; set; }
}