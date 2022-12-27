namespace CourseLibrary.API.Contracts.Authors;

public class AuthorForUpdate
{
    public Guid Id { get; set; }
    public string MainCategory { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = string.Empty;
}
