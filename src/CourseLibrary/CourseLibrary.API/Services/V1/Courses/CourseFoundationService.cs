using CourseLibrary.API.Brokers.Logging;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Validators.Courses;

namespace CourseLibrary.API.Services.V1.Courses;

internal sealed class CourseFoundationService : ICourseFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<CourseFoundationService> _loggingBroker;

    public CourseFoundationService(IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<CourseFoundationService> loggingBroker)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
    }

    public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(course, new CourseValidator(true));

        return await _storageBroker.InsertCourseAsync(course, cancellationToken);
    }

    public async Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(course, new CourseValidator(false));

        return await _storageBroker.UpdateCourseAsync(course, cancellationToken);
    }

    public async Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateParameter(courseId, nameof(courseId));

        Course? maybeCourse = await _storageBroker.SelectCourseByIdAsync(courseId, cancellationToken);
        _servicesLogicValidator.ValidateStorageEntity<Course>(maybeCourse, courseId);

        bool deleted = await _storageBroker.DeleteCourseAsync(maybeCourse!, cancellationToken);

        if (!deleted)
        {
            string exceptionMsg = StaticData.ExceptionMessages.NoRowsWasEffectedByDeleting(nameof(Course), courseId);
            throw new Exception(exceptionMsg);
        }
    }

    public Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken) =>
        _storageBroker.DeleteCoursesByAuthorIdAsync(authorId, cancellationToken);

    public async ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateParameter(courseId, nameof(courseId));

        Course? storageCourse = await _storageBroker.SelectCourseByIdAsync(courseId, cancellationToken);
        _servicesLogicValidator.ValidateStorageEntity<Course>(storageCourse, courseId);

        return storageCourse!;
    }

    public IQueryable<Course> RetrieveAllCourses()
    {
        IQueryable<Course> storageCourses = _storageBroker.SelectAllCourses();

        if (!storageCourses.Any())
        {
            _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
        }

        return storageCourses;
    }
}