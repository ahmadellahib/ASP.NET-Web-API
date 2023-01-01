using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Contracts.Authors;

public class AuthorForUpdate
{
    public Guid Id { get; set; }
    public Guid MainCategoryId { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator Author(AuthorForUpdate authorForUpdate) => new()
    {
        Id = authorForUpdate.Id,
        MainCategoryId = authorForUpdate.MainCategoryId,
        ConcurrencyStamp = authorForUpdate.ConcurrencyStamp
    };
}