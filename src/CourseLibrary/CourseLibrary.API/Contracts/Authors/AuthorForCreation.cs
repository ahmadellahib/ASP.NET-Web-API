namespace CourseLibrary.API.Contracts.Authors;

public class AuthorForCreation
{
    public Guid UserId { get; set; }
    public string MainCategory { get; set; } = string.Empty;
}