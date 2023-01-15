using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Pagination;

namespace CourseLibrary.API.Services.V1.Users;

internal sealed class UserOrchestrationService : IUserOrchestrationService
{
    private readonly IUserProcessingService _userProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly IServicesExceptionsLogger<UserOrchestrationService> _servicesExceptionsLogger;

    public UserOrchestrationService(IUserProcessingService userProcessingService,
        IServicesLogicValidator servicesLogicValidator,
        IServicesExceptionsLogger<UserOrchestrationService> servicesExceptionsLogger)
    {
        _userProcessingService = userProcessingService ?? throw new ArgumentNullException(nameof(userProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public Task<User> CreateUserAsync(User user, CancellationToken cancellationToken) =>
        _userProcessingService.CreateUserAsync(user, cancellationToken);

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            User storageUser = await _userProcessingService.RetrieveUserByIdAsync(user.Id, cancellationToken);
            _servicesLogicValidator.ValidateEntityConcurrency<User>(user, storageUser);

            return await _userProcessingService.ModifyUserAsync(user, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _userProcessingService.RetrieveUserByIdAsync(userId, cancellationToken);

    public IEnumerable<User> RetrieveAllUsers() =>
        _userProcessingService.RetrieveAllUsers();

    public PagedList<User> SearchUsers(UserResourceParameters userResourceParameters)
    {
        try
        {
            IQueryable<User> users = _userProcessingService.SearchUsers(userResourceParameters);

            return PagedList<User>.Create(users, userResourceParameters.PageNumber, userResourceParameters.PageSize);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    private Exception HandleException(Exception exception)
    {
        throw exception switch
        {
            ResourceParametersException or CancellationException or ValidationException or IDependencyException or IServiceException => exception,
            EntityConcurrencyException<User> => _servicesExceptionsLogger.CreateAndLogValidationException(exception),
            _ => _servicesExceptionsLogger.CreateAndLogServiceException(exception),
        };
    }
}
