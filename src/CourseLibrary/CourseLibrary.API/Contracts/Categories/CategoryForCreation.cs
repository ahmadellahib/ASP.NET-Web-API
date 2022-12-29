namespace CourseLibrary.API.Contracts.Categories;

public class CategoryForCreation
{
    public string Name { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
}
