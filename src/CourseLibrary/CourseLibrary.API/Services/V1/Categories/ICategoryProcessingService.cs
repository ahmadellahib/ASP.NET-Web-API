using CourseLibrary.API.Models.Categories;

namespace CategoryLibrary.API.Services.V1.Categories;

public interface ICategoryProcessingService
{
    Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken);

    Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken);

    ValueTask<Category> RetrieveCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken);

    IQueryable<Category> RetrieveAllCategories();
}