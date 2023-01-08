using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Validators.Users;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.V1.Users;

internal sealed class UserFoundationService : IUserFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<UserFoundationService> _loggingBroker;
    private readonly IServicesExceptionsLogger<UserFoundationService> _servicesExceptionsLogger;

    public UserFoundationService(IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<UserFoundationService> loggingBroker,
        IServicesExceptionsLogger<UserFoundationService> servicesExceptionsLogger)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(user, new UserValidator(true));

            return await _storageBroker.InsertUserAsync(user, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(user, new UserValidator(false));

            return await _storageBroker.UpdateUserAsync(user, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateParameter(userId, nameof(userId));

            User? storageUser = await _storageBroker.SelectUserByIdAsync(userId, cancellationToken);
            _servicesLogicValidator.ValidateStorageEntity<User>(storageUser, userId);

            return storageUser!;
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public IQueryable<User> RetrieveAllUsers()
    {
        try
        {
            IQueryable<User> storageUsers = _storageBroker.SelectAllUsers();

            if (!storageUsers.Any())
            {
                _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
            }

            return storageUsers;
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
            case NotFoundEntityException<User>:
                throw _servicesExceptionsLogger.CreateAndLogValidationException(exception);
            case InvalidEntityException<User>:
                throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
            case SqlException:
                throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(exception);
            case DbUpdateConcurrencyException:
                LockedEntityException<User> lockedEntityException = new(exception);

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