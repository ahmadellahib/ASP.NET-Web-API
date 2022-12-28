using CourseLibrary.API.Extensions;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Services.V1.PropertyMappings;
using CourseLibrary.API.Services.V1.Users;

namespace CourseLibrary.API.Services.V1.Courses;

internal sealed class CourseProcessingService : ICourseProcessingService
{
    private readonly ICourseFoundationService _courseFoundationService;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IServicesExceptionsLogger<CourseProcessingService> _servicesExceptionsLogger;

    public CourseProcessingService(ICourseFoundationService courseFoundationService,
        IPropertyMappingService propertyMappingService,
        IServicesExceptionsLogger<CourseProcessingService> servicesExceptionsLogger)
    {
        _courseFoundationService = courseFoundationService ?? throw new ArgumentNullException(nameof(courseFoundationService));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        try
        {
            course.Id = Guid.NewGuid();
            course.CreatedDate = DateTimeOffset.UtcNow;
            course.UpdatedDate = DateTimeOffset.UtcNow;
            course.UpdatedById = course.CreatedById;
            course.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _courseFoundationService.CreateCourseAsync(course, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<UserFoundationService>) { throw; }
        catch (ServiceException<UserFoundationService>) { throw; }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        try
        {
            course.UpdatedDate = DateTimeOffset.UtcNow;
            course.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _courseFoundationService.ModifyCourseAsync(course, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<UserFoundationService>) { throw; }
        catch (ServiceException<UserFoundationService>) { throw; }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
       await _courseFoundationService.RemoveCourseByIdAsync(courseId, cancellationToken);

    public async Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken) =>
        await _courseFoundationService.RemoveCoursesByAuthorIdAsync(authorId, cancellationToken);

    public async ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
        await _courseFoundationService.RetrieveCourseByIdAsync(courseId, cancellationToken);

    public IQueryable<Course> RetrieveAllCourses() =>
        _courseFoundationService.RetrieveAllCourses();

    public IQueryable<Course> SearchCourses(CourseResourceParameters courseResourceParameters)
    {
        try
        {
            if (!_propertyMappingService.ValidMappingExistsFor<Course, Course>(courseResourceParameters.OrderBy))
            {
                throw new ResourceParametersException($"Order by '{courseResourceParameters.OrderBy}' is not valid property for Course.");
            }

            IQueryable<Course> collection = RetrieveAllCourses();

            if (!string.IsNullOrEmpty(courseResourceParameters.SearchQuery))
            {
                courseResourceParameters.SearchQuery = courseResourceParameters.SearchQuery.Trim();
                collection = collection.Where(x => x.Title.Contains(courseResourceParameters.SearchQuery));
            }

            if (!string.IsNullOrWhiteSpace(courseResourceParameters.OrderBy))
            {
                Dictionary<string, PropertyMappingValue> coursePropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<Course, Course>();
                collection = collection.ApplySort(courseResourceParameters.OrderBy, coursePropertyMappingDictionary);
            }

            return collection;
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}
