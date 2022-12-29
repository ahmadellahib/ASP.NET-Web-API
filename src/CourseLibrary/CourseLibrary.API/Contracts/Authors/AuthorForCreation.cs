namespace CourseLibrary.API.Contracts.Authors;

public class AuthorForCreation
{
    public Guid UserId { get; set; }
    public Guid MainCategoryId { get; set; }
}