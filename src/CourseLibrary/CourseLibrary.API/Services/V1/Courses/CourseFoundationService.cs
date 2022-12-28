using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Services.V1.PropertyMappings;
using CourseLibrary.API.Validators.Courses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.V1.Courses;

internal sealed class CourseFoundationService : ICourseFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<CourseFoundationService> _loggingBroker;
    private readonly IServicesExceptionsLogger<CourseFoundationService> _servicesExceptionsLogger;

    public CourseFoundationService(IStorageBroker storageBroker,
        IPropertyMappingService propertyMappingService,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<CourseFoundationService> loggingBroker,
        IServicesExceptionsLogger<CourseFoundationService> servicesExceptionsLogger)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
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
        catch (InvalidEntityException<Course> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<Course> lockedEntityException = new(dbUpdateConcurrencyException);

            throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            throw _servicesExceptionsLogger.CreateAndLogDependencyException(dbUpdateException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(course, new CourseValidator(false));

            return await _storageBroker.UpdateCourseAsync(course, cancellationToken);
        }
        catch (InvalidEntityException<Course> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<Course> lockedEntityException = new(dbUpdateConcurrencyException);

            throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            throw _servicesExceptionsLogger.CreateAndLogDependencyException(dbUpdateException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
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
        catch (InvalidParameterException invalidIdException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(invalidIdException);
        }
        catch (NotFoundEntityException<Course> notFoundEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(notFoundEntityException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
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
        catch (InvalidParameterException invalidIdException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(invalidIdException);
        }
        catch (NotFoundEntityException<Course> notFoundEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(notFoundEntityException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
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
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}