using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Services;

namespace CategoryLibrary.API.Services.V1.Categories;

internal sealed class CategoryOrchestrationService : ICategoryOrchestrationService
{
    private readonly ICategoryProcessingService _categoryProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly IServicesExceptionsLogger<CategoryOrchestrationService> _servicesExceptionsLogger;

    public CategoryOrchestrationService(ICategoryProcessingService categoryProcessingService,
        IServicesLogicValidator servicesLogicValidator,
        IServicesExceptionsLogger<CategoryOrchestrationService> servicesExceptionsLogger)
    {
        _categoryProcessingService = categoryProcessingService ?? throw new ArgumentNullException(nameof(categoryProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            if (_categoryProcessingService.RetrieveAllCategories().Where(x => x.Name == category.Name).Any())
            {
                throw new CategoryWithSameNameAlreadyExistsException();
            }

            return await _categoryProcessingService.CreateCategoryAsync(category, cancellationToken);
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
            if (_categoryProcessingService.RetrieveAllCategories().Where(x => x.Id != category.Id && x.Name == category.Name).Any())
            {
                throw new CategoryWithSameNameAlreadyExistsException();
            }

            Category storageCategory = _categoryProcessingService.RetrieveCategoryById(category.Id);
            _servicesLogicValidator.ValidateEntityConcurrency<Category>(category, storageCategory);

            category.CreatedById = storageCategory.CreatedById;
            category.CreatedDate = storageCategory.CreatedDate;

            return await _categoryProcessingService.ModifyCategoryAsync(category, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public Category RetrieveCategoryById(Guid categoryId) =>
        _categoryProcessingService.RetrieveCategoryById(categoryId);

    public IEnumerable<Category> RetrieveAllCategories() =>
        _categoryProcessingService.RetrieveAllCategories();

    private Exception HandleException(Exception exception)
    {
        throw exception switch
        {
            CancellationException or ValidationException or IDependencyException or IServiceException => exception,
            CategoryWithSameNameAlreadyExistsException or EntityConcurrencyException<Category> => _servicesExceptionsLogger.CreateAndLogValidationException(exception),
            _ => _servicesExceptionsLogger.CreateAndLogServiceException(exception),
        };
    }
}