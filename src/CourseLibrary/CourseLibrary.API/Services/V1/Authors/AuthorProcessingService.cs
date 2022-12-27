using CourseLibrary.API.Extensions;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Services.V1.PropertyMappings;

namespace CourseLibrary.API.Services.V1.Authors;

public class AuthorProcessingService : IAuthorProcessingService
{
    private readonly IAuthorFoundationService _authorFoundationService;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IServicesExceptionsLogger<AuthorProcessingService> _servicesExceptionsLogger;

    public AuthorProcessingService(IAuthorFoundationService authorFoundationService,
        IPropertyMappingService propertyMappingService,
        IServicesExceptionsLogger<AuthorProcessingService> servicesExceptionsLogger)
    {
        _authorFoundationService = authorFoundationService ?? throw new ArgumentNullException(nameof(authorFoundationService));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        try
        {
            author.Id = Guid.NewGuid();
            author.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _authorFoundationService.CreateAuthorAsync(author, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<AuthorFoundationService>) { throw; }
        catch (ServiceException<AuthorFoundationService>) { throw; }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        try
        {
            author.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _authorFoundationService.ModifyAuthorAsync(author, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<AuthorFoundationService>) { throw; }
        catch (ServiceException<AuthorFoundationService>) { throw; }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken) =>
       await _authorFoundationService.RemoveAuthorByIdAsync(authorId, cancellationToken);

    public async ValueTask<Author> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken) =>
      await _authorFoundationService.RetrieveAuthorByIdAsync(authorId, cancellationToken);

    public IQueryable<Author> RetrieveAllAuthors() =>
        _authorFoundationService.RetrieveAllAuthors();

    public IQueryable<Author> SearchAuthors(AuthorResourceParameters authorResourceParameters)
    {
        try
        {
            if (!_propertyMappingService.ValidMappingExistsFor<Author, Author>(authorResourceParameters.OrderBy))
            {
                throw new ResourceParametersException($"Order by '{authorResourceParameters.OrderBy}' is not valid property for Author.");
            }

            IQueryable<Author> collection = RetrieveAllAuthors();

            if (!string.IsNullOrEmpty(authorResourceParameters.SearchQuery))
            {
                authorResourceParameters.SearchQuery = authorResourceParameters.SearchQuery.Trim();
                collection = collection.Where(x => x.MainCategory.Contains(authorResourceParameters.SearchQuery));
            }

            if (!string.IsNullOrWhiteSpace(authorResourceParameters.OrderBy))
            {
                Dictionary<string, PropertyMappingValue> authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<Author, Author>();
                collection = collection.ApplySort(authorResourceParameters.OrderBy, authorPropertyMappingDictionary);
            }

            return collection;
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}
