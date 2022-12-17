using CourseLibrary.API.Models.Exceptions;

namespace CourseLibrary.API.Services;

public interface IServicesExceptionsLogger<T> where T : class
{
    DependencyException<T> CreateAndLogCriticalConflictException(Exception exception);
    DependencyException<T> CreateAndLogCriticalDependencyException(Exception exception);
    DependencyException<T> CreateAndLogDependencyException(Exception exception);
    ServiceException<T> CreateAndLogServiceException(Exception exception);
    ValidationException CreateValidationException(Exception exception);
    ValidationException CreateAndLogValidationException(Exception exception);
    CancellationException CreateAndLogCancellationException(Exception exception);
}
