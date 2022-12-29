namespace CourseLibrary.API.Contracts.Authors;

public class AuthorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MainCategoryId { get; set; }
    public string MainCategory { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = string.Empty;
}
