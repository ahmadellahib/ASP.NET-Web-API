using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Authors;

namespace CourseLibrary.API.Services.V1.Users;

public class UserOrchestrationService : IUserOrchestrationService
{
    private readonly IUserProcessingService _userProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly IAuthorOrchestrationService _authorOrchestrationService;
    private readonly IServicesExceptionsLogger<UserOrchestrationService> _servicesExceptionsLogger;

    public UserOrchestrationService(IUserProcessingService userProcessingService,
        IServicesLogicValidator servicesLogicValidator,
        IAuthorOrchestrationService authorOrchestrationService,
        IServicesExceptionsLogger<UserOrchestrationService> servicesExceptionsLogger)
    {
        _userProcessingService = userProcessingService ?? throw new ArgumentNullException(nameof(userProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _authorOrchestrationService = authorOrchestrationService ?? throw new ArgumentNullException(nameof(authorOrchestrationService));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken) =>
        await _userProcessingService.CreateUserAsync(user, cancellationToken);

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            User storageUser = await _userProcessingService.RetrieveUserByIdAsync(user.Id, cancellationToken);
            _servicesLogicValidator.ValidateEntityConcurrency<User>(user, storageUser);

            return await _userProcessingService.ModifyUserAsync(user, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<UserFoundationService>) { throw; }
        catch (ServiceException<UserFoundationService>) { throw; }
        catch (EntityConcurrencyException<User> entityConcurrencyException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(entityConcurrencyException);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await _userProcessingService.RetrieveUserByIdAsync(userId, cancellationToken);

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
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}
