namespace CourseLibrary.API.Contracts.Authors;

public class AuthorForUpdate
{
    public Guid Id { get; set; }
    public Guid MainCategoryId { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;
}
