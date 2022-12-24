using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Extensions;
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
    private readonly ServicesExceptionsLogger<UserFoundationService> _servicesExceptionsLogger;

    public UserFoundationService(IStorageBroker storageBroker,
        IPropertyMappingService propertyMappingService,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<UserFoundationService> loggingBroker,
        ServicesExceptionsLogger<UserFoundationService> servicesExceptionsLogger)
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

    public IQueryable<User> SearchUsers(UserResourceParameters userResourceParameters)
    {
        try
        {
            if (!_propertyMappingService.ValidMappingExistsFor<User, User>(userResourceParameters.OrderBy))
            {
                throw new ResourceParametersException($"Order by '{userResourceParameters.OrderBy}' is not valid property for User.");
            }

            IQueryable<User> collection = RetrieveAllUsers();

            if (!string.IsNullOrEmpty(userResourceParameters.SearchQuery))
            {
                userResourceParameters.SearchQuery = userResourceParameters.SearchQuery.Trim();
                collection = collection.Where(x => x.FirstName.Contains(userResourceParameters.SearchQuery) ||
                    x.LastName.Contains(userResourceParameters.SearchQuery));
            }

            if (!string.IsNullOrWhiteSpace(userResourceParameters.OrderBy))
            {
                Dictionary<string, PropertyMappingValue> userPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<User, User>();
                collection = collection.ApplySort(userResourceParameters.OrderBy, userPropertyMappingDictionary);
            }

            return collection;
        }
        catch (Exception exception) { throw _servicesExceptionsLogger.CreateAndLogServiceException(exception); }
    }
}
