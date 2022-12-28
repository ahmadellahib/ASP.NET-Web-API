using CourseLibrary.API.Extensions;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Services.V1.PropertyMappings;

namespace CourseLibrary.API.Services.V1.Users;

internal sealed class UserProcessingService : IUserProcessingService
{
    private readonly IUserFoundationService _userFoundationService;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IServicesExceptionsLogger<UserProcessingService> _servicesExceptionsLogger;

    public UserProcessingService(IUserFoundationService userFoundationService,
        IPropertyMappingService propertyMappingService,
        IServicesExceptionsLogger<UserProcessingService> servicesExceptionsLogger)
    {
        _userFoundationService = userFoundationService ?? throw new ArgumentNullException(nameof(userFoundationService));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            user.Id = Guid.NewGuid();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _userFoundationService.CreateUserAsync(user, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<UserFoundationService>) { throw; }
        catch (ServiceException<UserFoundationService>) { throw; }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _userFoundationService.ModifyUserAsync(user, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<UserFoundationService>) { throw; }
        catch (ServiceException<UserFoundationService>) { throw; }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await _userFoundationService.RetrieveUserByIdAsync(userId, cancellationToken);

    public IQueryable<User> RetrieveAllUsers() =>
        _userFoundationService.RetrieveAllUsers();

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
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}
