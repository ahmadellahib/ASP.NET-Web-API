using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Contracts.Authors;

public class AuthorUpdatedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MainCategoryId { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator AuthorUpdatedDto(Author author) => new()
    {
        Id = author.Id,
        UserId = author.UserId,
        MainCategoryId = author.MainCategoryId,
        ConcurrencyStamp = author.ConcurrencyStamp
    };
}
