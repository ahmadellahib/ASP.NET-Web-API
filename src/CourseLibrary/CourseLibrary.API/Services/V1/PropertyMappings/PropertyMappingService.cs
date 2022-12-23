namespace CourseLibrary.API.Services.V1.PropertyMappings;

public partial class PropertyMappingService : IPropertyMappingService
{
    private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

    public PropertyMappingService()
    {
        AddPropertyMappings();
    }

    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
    {
        // get matching mapping
        IEnumerable<PropertyMapping<TSource, TDestination>> matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

        if (matchingMapping.Count() == 1)
        {
            return matchingMapping.First()._mappingDictionary;
        }

        throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
    }

    public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
    {
        Dictionary<string, PropertyMappingValue> propertyMapping = GetPropertyMapping<TSource, TDestination>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            return true;
        }

        // the string is separated by ",", so we split it.
        string[] fieldsAfterSplit = fields.Split(',');

        // run through the fields clauses
        foreach (string field in fieldsAfterSplit)
        {
            // trim
            string trimmedField = field.Trim();

            // remove everything after the first " " - if the fields
            // are coming from an orderBy string, this part must be
            // ignored
            int indexOfFirstSpace = trimmedField.IndexOf(" ");
            string propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

            // find the matching property
            if (!propertyMapping.ContainsKey(propertyName))
            {
                return false;
            }
        }
        return true;
    }
}