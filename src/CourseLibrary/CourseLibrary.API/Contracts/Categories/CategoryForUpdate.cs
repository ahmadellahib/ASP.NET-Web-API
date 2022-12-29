namespace CourseLibrary.API.Contracts.Categories;

public class CategoryForUpdate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UpdatedById { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;
}
