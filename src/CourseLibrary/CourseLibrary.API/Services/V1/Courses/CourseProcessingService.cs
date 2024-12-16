using CourseLibrary.API.Extensions;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Services.V1.PropertyMappings;

namespace CourseLibrary.API.Services.V1.Courses;

internal sealed class CourseProcessingService : ICourseProcessingService
{
    private readonly ICourseFoundationService _courseFoundationService;
    private readonly IPropertyMappingService _propertyMappingService;

    public CourseProcessingService(ICourseFoundationService courseFoundationService, IPropertyMappingService propertyMappingService)
    {
        _courseFoundationService = courseFoundationService ?? throw new ArgumentNullException(nameof(courseFoundationService));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
    }

    public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        course.Id = Guid.NewGuid();
        course.CreatedDate = DateTimeOffset.UtcNow;
        course.UpdatedDate = course.CreatedDate;
        course.UpdatedById = course.CreatedById;
        course.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _courseFoundationService.CreateCourseAsync(course, cancellationToken);
    }

    public async Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        course.UpdatedDate = DateTimeOffset.UtcNow;
        course.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _courseFoundationService.ModifyCourseAsync(course, cancellationToken);
    }

    public Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
       _courseFoundationService.RemoveCourseByIdAsync(courseId, cancellationToken);

    public Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken) =>
        _courseFoundationService.RemoveCoursesByAuthorIdAsync(authorId, cancellationToken);

    public ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
        _courseFoundationService.RetrieveCourseByIdAsync(courseId, cancellationToken);

    public IQueryable<Course> RetrieveAllCourses() =>
        _courseFoundationService.RetrieveAllCourses();

    public IQueryable<Course> SearchCourses(CourseResourceParameters courseResourceParameters)
    {
        if (!_propertyMappingService.ValidMappingExistsFor<Course, Course>(courseResourceParameters.OrderBy))
        {
            throw new ResourceParametersException($"Order by '{courseResourceParameters.OrderBy}' is not valid property for Course.");
        }

        IQueryable<Course> collection = _courseFoundationService.RetrieveAllCourses();

        if (!string.IsNullOrEmpty(courseResourceParameters.SearchQuery))
        {
            courseResourceParameters.SearchQuery = courseResourceParameters.SearchQuery.Trim();
            collection = collection.Where(x => x.Title.Contains(courseResourceParameters.SearchQuery));
        }

        if (string.IsNullOrWhiteSpace(courseResourceParameters.OrderBy))
        {
            return collection;
        }

        Dictionary<string, PropertyMappingValue> coursePropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<Course, Course>();
        collection = collection.ApplySort(courseResourceParameters.OrderBy, coursePropertyMappingDictionary);

        return collection;
    }
}
