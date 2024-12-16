using CourseLibrary.API.Extensions;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Services.V1.PropertyMappings;

namespace CourseLibrary.API.Services.V1.Authors;

internal sealed class AuthorProcessingService : IAuthorProcessingService
{
    private readonly IAuthorFoundationService _authorFoundationService;
    private readonly IPropertyMappingService _propertyMappingService;

    public AuthorProcessingService(IAuthorFoundationService authorFoundationService, IPropertyMappingService propertyMappingService)
    {
        _authorFoundationService = authorFoundationService ?? throw new ArgumentNullException(nameof(authorFoundationService));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
    }

    public async Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        author.Id = Guid.NewGuid();
        author.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _authorFoundationService.CreateAuthorAsync(author, cancellationToken);
    }

    public async Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        author.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _authorFoundationService.ModifyAuthorAsync(author, cancellationToken);
    }

    public Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken) =>
       _authorFoundationService.RemoveAuthorByIdAsync(authorId, cancellationToken);

    public ValueTask<Author> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken) =>
      _authorFoundationService.RetrieveAuthorByIdAsync(authorId, cancellationToken);

    public IQueryable<Author> RetrieveAllAuthors() =>
        _authorFoundationService.RetrieveAllAuthors();

    public IQueryable<Author> SearchAuthors(AuthorResourceParameters authorResourceParameters)
    {
        if (!_propertyMappingService.ValidMappingExistsFor<Author, Author>(authorResourceParameters.OrderBy))
        {
            throw new ResourceParametersException($"Order by '{authorResourceParameters.OrderBy}' is not valid property for Author.");
        }

        IQueryable<Author> collection = _authorFoundationService.RetrieveAllAuthors();

        if (string.IsNullOrWhiteSpace(authorResourceParameters.OrderBy))
        {
            return collection;
        }

        Dictionary<string, PropertyMappingValue> authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<Author, Author>();
        collection = collection.ApplySort(authorResourceParameters.OrderBy, authorPropertyMappingDictionary);

        return collection;
    }
}
