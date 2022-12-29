namespace CourseLibrary.API.Models.Exceptions;

public class CategoryWithSameNameAlreadyExists : Exception
{
    public CategoryWithSameNameAlreadyExists() : base("Category with same name already exists.") { }
}
