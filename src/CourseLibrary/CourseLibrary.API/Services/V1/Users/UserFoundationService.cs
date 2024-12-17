using CourseLibrary.API.Brokers.Logging;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Validators.Users;

namespace CourseLibrary.API.Services.V1.Users;

internal sealed class UserFoundationService : IUserFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<UserFoundationService> _loggingBroker;

    public UserFoundationService(IStorageBroker storageBroker, IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<UserFoundationService> loggingBroker)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(user, new UserValidator(true));

        return await _storageBroker.InsertUserAsync(user, cancellationToken);
    }

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(user, new UserValidator(false));

        return await _storageBroker.UpdateUserAsync(user, cancellationToken);
    }

    public async ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateParameter(userId, nameof(userId));

        User? storageUser = await _storageBroker.SelectUserByIdAsync(userId, cancellationToken);
        _servicesLogicValidator.ValidateStorageEntity<User>(storageUser, userId);

        return storageUser!;
    }

    public IQueryable<User> RetrieveAllUsers()
    {
        IQueryable<User> storageUsers = _storageBroker.SelectAllUsers();

        if (!storageUsers.Any())
        {
            _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
        }

        return storageUsers;
    }
}