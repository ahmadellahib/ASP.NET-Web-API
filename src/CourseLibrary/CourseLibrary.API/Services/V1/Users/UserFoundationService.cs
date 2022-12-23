using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Validators.Users;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.V1.Users;

public class UserFoundationService : IUserFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<UserFoundationService> _loggingBroker;
    private readonly ServicesExceptionsLogger<UserFoundationService> _servicesExceptionsLogger;

    public UserFoundationService(IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<UserFoundationService> loggingBroker,
        ServicesExceptionsLogger<UserFoundationService> servicesExceptionsLogger)
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

            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _storageBroker.InsertUserAsync(user, cancellationToken);
        }
        catch (InvalidEntityException<User> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<User> lockedEntityException = new(dbUpdateConcurrencyException);

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

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(user, new UserValidator(false));

            User? maybeUser = await _storageBroker.SelectUserByIdAsync(user.Id, cancellationToken);
            _servicesLogicValidator.ValidateStorageEntity<User>(maybeUser, user.Id);
            _servicesLogicValidator.ValidateEntityConcurrency<User>(user, maybeUser!);

            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _storageBroker.UpdateUserAsync(user, cancellationToken);
        }
        catch (InvalidEntityException<User> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (NotFoundEntityException<User> notFoundEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(notFoundEntityException);
        }
        catch (EntityConcurrencyException<User> entityConcurrencyException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(entityConcurrencyException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<User> lockedEntityException = new(dbUpdateConcurrencyException);

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

    public async Task RemoveUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateParameter(userId, nameof(userId));

            User? maybeUser = await _storageBroker.SelectUserByIdAsync(userId, cancellationToken);
            _servicesLogicValidator.ValidateStorageEntity<User>(maybeUser, userId);

            bool deleted = await _storageBroker.DeleteUserAsync(maybeUser!, cancellationToken);

            if (!deleted)
            {
                string exceptionMsg = StaticData.ExceptionMessages.NoRowsWasEffectedByDeleting(nameof(User), userId);
                throw new Exception(exceptionMsg);
            }
        }
        catch (InvalidParameterException invalidIdException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(invalidIdException);
        }
        catch (NotFoundEntityException<User> notFoundEntityException)
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
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
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
        catch (InvalidParameterException invalidIdException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(invalidIdException);
        }
        catch (NotFoundEntityException<User> notFoundEntityException)
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
}
