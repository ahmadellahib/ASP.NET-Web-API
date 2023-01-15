using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.Contracts.Categories;

public class CategoryForUpdate
{
    public string Name { get; set; } = string.Empty;
    public Guid UpdatedById { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator Category(CategoryForUpdate categoryForUpdate) => new()
    {
        Name = categoryForUpdate.Name,
        UpdatedById = categoryForUpdate.UpdatedById,
        ConcurrencyStamp = categoryForUpdate.ConcurrencyStamp
    };
}