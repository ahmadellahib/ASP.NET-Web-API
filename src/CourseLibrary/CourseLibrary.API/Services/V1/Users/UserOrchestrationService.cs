using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Pagination;

namespace CourseLibrary.API.Services.V1.Users;

internal sealed class UserOrchestrationService : IUserOrchestrationService
{
    private readonly IUserProcessingService _userProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;

    public UserOrchestrationService(IUserProcessingService userProcessingService, IServicesLogicValidator servicesLogicValidator)
    {
        _userProcessingService = userProcessingService ?? throw new ArgumentNullException(nameof(userProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
    }

    public Task<User> CreateUserAsync(User user, CancellationToken cancellationToken) =>
        _userProcessingService.CreateUserAsync(user, cancellationToken);

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        User storageUser = await _userProcessingService.RetrieveUserByIdAsync(user.Id, cancellationToken);
        _servicesLogicValidator.ValidateEntityConcurrency<User>(user, storageUser);

        return await _userProcessingService.ModifyUserAsync(user, cancellationToken);
    }

    public ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _userProcessingService.RetrieveUserByIdAsync(userId, cancellationToken);

    public IEnumerable<User> RetrieveAllUsers() =>
        _userProcessingService.RetrieveAllUsers();

    public PagedList<User> SearchUsers(UserResourceParameters userResourceParameters)
    {
        IQueryable<User> users = _userProcessingService.SearchUsers(userResourceParameters);

        return PagedList<User>.Create(users, userResourceParameters.PageNumber, userResourceParameters.PageSize);
    }
}
