using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.Contracts.Categories;

public class CategoryForCreation
{
    public string Name { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }

    public static explicit operator Category(CategoryForCreation categoryForCreation) => new()
    {
        Name = categoryForCreation.Name,
        CreatedById = categoryForCreation.CreatedById
    };
}