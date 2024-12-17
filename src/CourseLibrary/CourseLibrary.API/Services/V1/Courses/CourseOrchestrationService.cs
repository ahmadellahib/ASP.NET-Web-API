using CourseLibrary.API.Diagnostics;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Pagination;

namespace CourseLibrary.API.Services.V1.Courses;

internal sealed class CourseOrchestrationService : ICourseOrchestrationService
{
    private readonly ICourseProcessingService _courseProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;

    public CourseOrchestrationService(ICourseProcessingService courseProcessingService, IServicesLogicValidator servicesLogicValidator)
    {
        _courseProcessingService = courseProcessingService ?? throw new ArgumentNullException(nameof(courseProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
    }

    public Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken) =>
        _courseProcessingService.CreateCourseAsync(course, cancellationToken);

    public async Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        Course storageCourse = await _courseProcessingService.RetrieveCourseByIdAsync(course.Id, cancellationToken);
        _servicesLogicValidator.ValidateEntityConcurrency<Course>(course, storageCourse);

        course.CreatedById = storageCourse.CreatedById;
        course.CreatedDate = storageCourse.CreatedDate;

        return await _courseProcessingService.ModifyCourseAsync(course, cancellationToken);
    }

    public Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
        _courseProcessingService.RemoveCourseByIdAsync(courseId, cancellationToken);

    public Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken) =>
        _courseProcessingService.RemoveCoursesByAuthorIdAsync(authorId, cancellationToken);

    public async ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        Course course = await _courseProcessingService.RetrieveCourseByIdAsync(courseId, cancellationToken);

        // log course metrics
        DiagnosticsConfig.CoursesVisited.Add(1,
            new KeyValuePair<string, object?>("course.id", course.Id),
            new KeyValuePair<string, object?>("course.title", course.Title),
            new KeyValuePair<string, object?>("course.author.id", course.AuthorId));
        return course;
    }


    public IEnumerable<Course> RetrieveAllCourses() =>
        _courseProcessingService.RetrieveAllCourses();

    public PagedList<Course> SearchCourses(CourseResourceParameters courseResourceParameters)
    {
        IQueryable<Course> courses = _courseProcessingService.SearchCourses(courseResourceParameters);

        return PagedList<Course>.Create(courses, courseResourceParameters.PageNumber, courseResourceParameters.PageSize);
    }
}
