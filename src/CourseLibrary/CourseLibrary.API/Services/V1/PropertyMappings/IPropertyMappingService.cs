namespace CourseLibrary.API.Services.V1.PropertyMappings;

public interface IPropertyMappingService
{
    Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();

    bool ValidMappingExistsFor<TSource, TDestination>(string fields);
}
