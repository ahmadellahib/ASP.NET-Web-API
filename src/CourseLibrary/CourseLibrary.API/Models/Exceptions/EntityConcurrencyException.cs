namespace CourseLibrary.API.Models.Exceptions;

public class EntityConcurrencyException<T> : Exception
{
    public EntityConcurrencyException()
        : base($"{typeof(T).Name}  is not concurrent with storage entity.") { }
}