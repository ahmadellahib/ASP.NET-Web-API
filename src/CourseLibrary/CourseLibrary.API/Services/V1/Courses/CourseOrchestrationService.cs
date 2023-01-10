using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Pagination;

namespace CourseLibrary.API.Services.V1.Courses;

internal sealed class CourseOrchestrationService : ICourseOrchestrationService
{
    private readonly ICourseProcessingService _courseProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly IServicesExceptionsLogger<CourseOrchestrationService> _servicesExceptionsLogger;

    public CourseOrchestrationService(ICourseProcessingService courseProcessingService,
        IServicesLogicValidator servicesLogicValidator,
        IServicesExceptionsLogger<CourseOrchestrationService> servicesExceptionsLogger)
    {
        _courseProcessingService = courseProcessingService ?? throw new ArgumentNullException(nameof(courseProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken) =>
        await _courseProcessingService.CreateCourseAsync(course, cancellationToken);

    public async Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        try
        {
            Course storageCourse = await _courseProcessingService.RetrieveCourseByIdAsync(course.Id, cancellationToken);
            _servicesLogicValidator.ValidateEntityConcurrency<Course>(course, storageCourse);

            course.CreatedById = storageCourse.CreatedById;
            course.CreatedDate = storageCourse.CreatedDate;

            return await _courseProcessingService.ModifyCourseAsync(course, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
        await _courseProcessingService.RemoveCourseByIdAsync(courseId, cancellationToken);

    public async Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken) =>
        await _courseProcessingService.RemoveCoursesByAuthorIdAsync(authorId, cancellationToken);

    public async ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
        await _courseProcessingService.RetrieveCourseByIdAsync(courseId, cancellationToken);

    public IEnumerable<Course> RetrieveAllCourses() =>
        _courseProcessingService.RetrieveAllCourses();

    public PagedList<Course> SearchCourses(CourseResourceParameters courseResourceParameters)
    {
        try
        {
            IQueryable<Course> courses = _courseProcessingService.SearchCourses(courseResourceParameters);

            return PagedList<Course>.Create(courses, courseResourceParameters.PageNumber, courseResourceParameters.PageSize);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    private Exception HandleException(Exception exception)
    {
        throw exception switch
        {
            ResourceParametersException or CancellationException or ValidationException or IDependencyException or IServiceException => exception,
            EntityConcurrencyException<Course> => _servicesExceptionsLogger.CreateAndLogValidationException(exception),
            _ => _servicesExceptionsLogger.CreateAndLogServiceException(exception),
        };
    }
}
