using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Services.V1.PropertyMappings;
using CourseLibrary.API.Validators.Users;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.V1.Users;

public class UserFoundationService : IUserFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly ILoggingBroker<UserFoundationService> _loggingBroker;
    private readonly IServicesExceptionsLogger<UserFoundationService> _servicesExceptionsLogger;

    public UserFoundationService(IStorageBroker storageBroker,
        IPropertyMappingService propertyMappingService,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<UserFoundationService> loggingBroker,
        IServicesExceptionsLogger<UserFoundationService> servicesExceptionsLogger)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
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

            return await _storageBroker.UpdateUserAsync(user, cancellationToken);
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
}