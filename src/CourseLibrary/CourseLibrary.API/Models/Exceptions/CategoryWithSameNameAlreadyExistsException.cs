namespace CourseLibrary.API.Models.Exceptions;

public class CategoryWithSameNameAlreadyExistsException : Exception
{
    public CategoryWithSameNameAlreadyExistsException() : base("Category with same name already exists.") { }
}
