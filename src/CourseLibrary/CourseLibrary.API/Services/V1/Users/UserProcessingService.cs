using CourseLibrary.API.Extensions;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Services.V1.PropertyMappings;

namespace CourseLibrary.API.Services.V1.Users;

internal sealed class UserProcessingService : IUserProcessingService
{
    private readonly IUserFoundationService _userFoundationService;
    private readonly IPropertyMappingService _propertyMappingService;

    public UserProcessingService(IUserFoundationService userFoundationService, IPropertyMappingService propertyMappingService)
    {
        _userFoundationService = userFoundationService ?? throw new ArgumentNullException(nameof(userFoundationService));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        user.Id = Guid.NewGuid();
        user.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _userFoundationService.CreateUserAsync(user, cancellationToken);
    }

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        user.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _userFoundationService.ModifyUserAsync(user, cancellationToken);
    }

    public ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _userFoundationService.RetrieveUserByIdAsync(userId, cancellationToken);

    public IQueryable<User> RetrieveAllUsers() =>
        _userFoundationService.RetrieveAllUsers();

    public IQueryable<User> SearchUsers(UserResourceParameters userResourceParameters)
    {
        if (!_propertyMappingService.ValidMappingExistsFor<User, User>(userResourceParameters.OrderBy))
        {
            throw new ResourceParametersException($"Order by '{userResourceParameters.OrderBy}' is not valid property for User.");
        }

        IQueryable<User> collection = _userFoundationService.RetrieveAllUsers();

        if (!string.IsNullOrEmpty(userResourceParameters.SearchQuery))
        {
            userResourceParameters.SearchQuery = userResourceParameters.SearchQuery.Trim();
            collection = collection.Where(x => x.FirstName.Contains(userResourceParameters.SearchQuery) ||
                                               x.LastName.Contains(userResourceParameters.SearchQuery));
        }

        if (string.IsNullOrWhiteSpace(userResourceParameters.OrderBy))
        {
            return collection;
        }

        Dictionary<string, PropertyMappingValue> userPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<User, User>();
        collection = collection.ApplySort(userResourceParameters.OrderBy, userPropertyMappingDictionary);

        return collection;
    }
}