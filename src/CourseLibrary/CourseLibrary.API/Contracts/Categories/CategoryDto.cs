using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.Contracts.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public Guid UpdatedById { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator CategoryDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        CreatedDate = category.CreatedDate,
        UpdatedDate = category.UpdatedDate,
        CreatedById = category.CreatedById,
        UpdatedById = category.UpdatedById,
        ConcurrencyStamp = category.ConcurrencyStamp
    };
}
