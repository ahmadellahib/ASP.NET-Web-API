using CategoryLibrary.API.Services.V1.Categories;
using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Exceptions;

namespace CourseLibrary.API.Services.V1.Categories;

internal sealed class CategoryProcessingService : ICategoryProcessingService
{
    private readonly ICategoryFoundationService _categoryFoundationService;
    private readonly IServicesExceptionsLogger<CategoryProcessingService> _servicesExceptionsLogger;

    public CategoryProcessingService(ICategoryFoundationService categoryFoundationService,
        IServicesExceptionsLogger<CategoryProcessingService> servicesExceptionsLogger)
    {
        _categoryFoundationService = categoryFoundationService ?? throw new ArgumentNullException(nameof(categoryFoundationService));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            category.Id = Guid.NewGuid();
            category.CreatedDate = DateTimeOffset.UtcNow;
            category.UpdatedDate = category.CreatedDate;
            category.UpdatedById = category.CreatedById;
            category.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _categoryFoundationService.CreateCategoryAsync(category, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            category.UpdatedDate = DateTimeOffset.UtcNow;
            category.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _categoryFoundationService.ModifyCategoryAsync(category, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public Category RetrieveCategoryById(Guid categoryId) =>
        _categoryFoundationService.RetrieveCategoryById(categoryId);

    public IEnumerable<Category> RetrieveAllCategories() =>
        _categoryFoundationService.RetrieveAllCategories();

    private Exception HandleException(Exception exception)
    {
        throw exception switch
        {
            CancellationException or ValidationException or IDependencyException or IServiceException => exception,
            _ => _servicesExceptionsLogger.CreateAndLogServiceException(exception),
        };
    }
}
