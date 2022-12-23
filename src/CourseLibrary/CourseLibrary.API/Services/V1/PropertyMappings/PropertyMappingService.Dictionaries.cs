using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Services.V1.PropertyMappings;

public partial class PropertyMappingService
{
    private void AddPropertyMappings()
    {
        _propertyMappings.Add(new PropertyMapping<User, User>(_userPropertyMapping));
        _propertyMappings.Add(new PropertyMapping<Author, Author>(_authorPropertyMapping));
        _propertyMappings.Add(new PropertyMapping<Course, Course>(_coursePropertyMapping));
    }

    private readonly Dictionary<string, PropertyMappingValue> _userPropertyMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(User.Id), new PropertyMappingValue(new List<string>() { nameof(User.Id) }) },
        { nameof(User.FirstName), new PropertyMappingValue(new List<string>() { nameof(User.FirstName) }) },
        { nameof(User.LastName), new PropertyMappingValue(new List<string>() { nameof(User.LastName) }) }
    };

    private readonly Dictionary<string, PropertyMappingValue> _authorPropertyMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Author.Id), new PropertyMappingValue(new List<string>() { nameof(Author.Id) }) },
        { nameof(Author.MainCategory), new PropertyMappingValue(new List<string>() { nameof(Author.MainCategory) }) }
    };

    private readonly Dictionary<string, PropertyMappingValue> _coursePropertyMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Course.Id), new PropertyMappingValue(new List<string>() { nameof(Course.Id) }) },
        { nameof(Course.Title), new PropertyMappingValue(new List<string>() { nameof(Course.Title) }) }
    };
}
