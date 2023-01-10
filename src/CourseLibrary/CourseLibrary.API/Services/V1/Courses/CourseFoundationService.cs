using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Validators.Courses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.V1.Courses;

internal sealed class CourseFoundationService : ICourseFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<CourseFoundationService> _loggingBroker;
    private readonly IServicesExceptionsLogger<CourseFoundationService> _servicesExceptionsLogger;

    public CourseFoundationService(IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<CourseFoundationService> loggingBroker,
        IServicesExceptionsLogger<CourseFoundationService> servicesExceptionsLogger)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(course, new CourseValidator(true));

            return await _storageBroker.InsertCourseAsync(course, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(course, new CourseValidator(false));

            return await _storageBroker.UpdateCourseAsync(course, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        try
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
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken) =>
        await _storageBroker.DeleteCoursesByAuthorIdAsync(authorId, cancellationToken);

    public async ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateParameter(courseId, nameof(courseId));

            Course? storageCourse = await _storageBroker.SelectCourseByIdAsync(courseId, cancellationToken);
            _servicesLogicValidator.ValidateStorageEntity<Course>(storageCourse, courseId);

            return storageCourse!;
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public IQueryable<Course> RetrieveAllCourses()
    {
        try
        {
            IQueryable<Course> storageCourses = _storageBroker.SelectAllCourses();

            if (!storageCourses.Any())
            {
                _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
            }

            return storageCourses;
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    private Exception HandleException(Exception exception)
    {
        switch (exception)
        {
            case InvalidParameterException:
            case NotFoundEntityException<Course>:
                throw _servicesExceptionsLogger.CreateAndLogValidationException(exception);
            case InvalidEntityException<Course>:
                throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
            case SqlException:
                throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(exception);
            case DbUpdateConcurrencyException:
                LockedEntityException<Course> lockedEntityException = new(exception);

                throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
            case DbUpdateException:
                throw _servicesExceptionsLogger.CreateAndLogDependencyException(exception);
            case TaskCanceledException:
            case OperationCanceledException:
                throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
            default:
                throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}