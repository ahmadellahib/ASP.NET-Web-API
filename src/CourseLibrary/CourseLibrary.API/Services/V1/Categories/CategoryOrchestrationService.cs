using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Exceptions;

namespace CourseLibrary.API.Services.V1.Categories;

internal sealed class CategoryOrchestrationService : ICategoryOrchestrationService
{
    private readonly ICategoryProcessingService _categoryProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;

    public CategoryOrchestrationService(ICategoryProcessingService categoryProcessingService, IServicesLogicValidator servicesLogicValidator)
    {
        _categoryProcessingService = categoryProcessingService ?? throw new ArgumentNullException(nameof(categoryProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
    }

    public async Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        if (_categoryProcessingService.RetrieveAllCategories().Any(x => x.Id != category.Id && x.Name == category.Name))
        {
            throw new CategoryWithSameNameAlreadyExistsException();
        }

        return await _categoryProcessingService.CreateCategoryAsync(category, cancellationToken);
    }

    public async Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        if (_categoryProcessingService.RetrieveAllCategories().Any(x => x.Id != category.Id && x.Name == category.Name))
        {
            throw new CategoryWithSameNameAlreadyExistsException();
        }

        Category storageCategory = _categoryProcessingService.RetrieveCategoryById(category.Id);
        _servicesLogicValidator.ValidateEntityConcurrency<Category>(category, storageCategory);

        category.CreatedById = storageCategory.CreatedById;
        category.CreatedDate = storageCategory.CreatedDate;

        return await _categoryProcessingService.ModifyCategoryAsync(category, cancellationToken);
    }

    public Category RetrieveCategoryById(Guid categoryId) =>
        _categoryProcessingService.RetrieveCategoryById(categoryId);

    public IEnumerable<Category> RetrieveAllCategories() =>
        _categoryProcessingService.RetrieveAllCategories();
}