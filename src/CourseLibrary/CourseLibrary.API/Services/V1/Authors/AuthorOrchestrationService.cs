using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Categories;
using CourseLibrary.API.Services.V1.Courses;

namespace CourseLibrary.API.Services.V1.Authors;

internal sealed class AuthorOrchestrationService : IAuthorOrchestrationService
{
    private readonly IAuthorProcessingService _authorProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ICourseOrchestrationService _courseOrchestrationService;
    private readonly ICategoryOrchestrationService _categoryOrchestrationService;

    public AuthorOrchestrationService(IAuthorProcessingService authorProcessingService,
        IServicesLogicValidator servicesLogicValidator,
        ICourseOrchestrationService courseOrchestrationService,
        ICategoryOrchestrationService categoryOrchestrationService)
    {
        _authorProcessingService = authorProcessingService ?? throw new ArgumentNullException(nameof(authorProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _courseOrchestrationService = courseOrchestrationService ?? throw new ArgumentNullException(nameof(courseOrchestrationService));
        _categoryOrchestrationService = categoryOrchestrationService ?? throw new ArgumentNullException(nameof(categoryOrchestrationService));
    }

    public async Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        Category? category = _categoryOrchestrationService.RetrieveAllCategories()
            .FirstOrDefault(c => c.Id == author.MainCategoryId);

        if (category is null)
            throw new InvalidParameterException(nameof(author.MainCategoryId), $"Category with id {author.MainCategoryId} does not exist.");

        return await _authorProcessingService.CreateAuthorAsync(author, cancellationToken);
    }

    public async Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        Author storageAuthor = await _authorProcessingService.RetrieveAuthorByIdAsync(author.Id, cancellationToken);
        _servicesLogicValidator.ValidateEntityConcurrency<Author>(author, storageAuthor);

        if (author.MainCategoryId != storageAuthor.MainCategoryId)
        {
            Category? category = _categoryOrchestrationService.RetrieveAllCategories()
                .FirstOrDefault(c => c.Id == author.MainCategoryId);

            if (category is null)
                throw new InvalidParameterException(nameof(author.MainCategoryId), $"Category with id {author.MainCategoryId} does not exist.");
        }

        author.UserId = storageAuthor.UserId;

        return await _authorProcessingService.ModifyAuthorAsync(author, cancellationToken);
    }

    public async Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        await _courseOrchestrationService.RemoveCoursesByAuthorIdAsync(authorId, cancellationToken);
        await _authorProcessingService.RemoveAuthorByIdAsync(authorId, cancellationToken);
    }

    public async ValueTask<Author> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories = _categoryOrchestrationService.RetrieveAllCategories();
        Author author = await _authorProcessingService.RetrieveAuthorByIdAsync(authorId, cancellationToken);

        author.MainCategory = categories.FirstOrDefault(category => category.Id == author.MainCategoryId)
                              ?? throw new InvalidOperationException("Main category not found.");

        return author;
    }

    public IEnumerable<Author> RetrieveAllAuthors()
    {
        Dictionary<Guid, Category> categoryDictionary = _categoryOrchestrationService.RetrieveAllCategories()
            .ToDictionary(category => category.Id);

        // Load authors and assign categories in a single pass
        List<Author> authors = _authorProcessingService.RetrieveAllAuthors().ToList();

        foreach (Author author in authors)
        {
            if (categoryDictionary.TryGetValue(author.MainCategoryId, out Category? mainCategory))
            {
                author.MainCategory = mainCategory;
            }
        }

        return authors;
    }

    public PagedList<Author> SearchAuthors(AuthorResourceParameters authorResourceParameters)
    {
        Dictionary<Guid, Category> categoryDictionary = _categoryOrchestrationService.RetrieveAllCategories()
            .ToDictionary(category => category.Id);

        IQueryable<Author> authors = _authorProcessingService.SearchAuthors(authorResourceParameters);
        PagedList<Author> pagedListAuthors = PagedList<Author>.Create(authors, authorResourceParameters.PageNumber, authorResourceParameters.PageSize);

        foreach (Author author in pagedListAuthors)
        {
            if (categoryDictionary.TryGetValue(author.MainCategoryId, out Category? mainCategory))
            {
                author.MainCategory = mainCategory;
            }
        }

        return pagedListAuthors;
    }
}