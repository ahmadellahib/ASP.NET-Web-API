using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Contracts.Authors;

public class AuthorForCreation
{
    public Guid UserId { get; set; }
    public Guid MainCategoryId { get; set; }

    public static explicit operator Author(AuthorForCreation authorForCreation) => new()
    {
        UserId = authorForCreation.UserId,
        MainCategoryId = authorForCreation.MainCategoryId
    };
}