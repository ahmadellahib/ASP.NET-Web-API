using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.Services.V1.Categories;

internal sealed class CategoryProcessingService : ICategoryProcessingService
{
    private readonly ICategoryFoundationService _categoryFoundationService;

    public CategoryProcessingService(ICategoryFoundationService categoryFoundationService)
    {
        _categoryFoundationService = categoryFoundationService ?? throw new ArgumentNullException(nameof(categoryFoundationService));
    }

    public async Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        category.Id = Guid.NewGuid();
        category.CreatedDate = DateTimeOffset.UtcNow;
        category.UpdatedDate = category.CreatedDate;
        category.UpdatedById = category.CreatedById;
        category.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _categoryFoundationService.CreateCategoryAsync(category, cancellationToken);
    }

    public async Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        category.UpdatedDate = DateTimeOffset.UtcNow;
        category.ConcurrencyStamp = Guid.NewGuid().ToString();

        return await _categoryFoundationService.ModifyCategoryAsync(category, cancellationToken);
    }

    public Category RetrieveCategoryById(Guid categoryId) =>
        _categoryFoundationService.RetrieveCategoryById(categoryId);

    public IEnumerable<Category> RetrieveAllCategories() =>
        _categoryFoundationService.RetrieveAllCategories();
}
