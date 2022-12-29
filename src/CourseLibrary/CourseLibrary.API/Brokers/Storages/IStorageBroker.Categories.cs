using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.Brokers.Storages;

internal partial interface IStorageBroker
{
    Task<Category> InsertCategoryAsync(Category category, CancellationToken cancellationToken);
    Task<Category> UpdateCategoryAsync(Category category, CancellationToken cancellationToken);
    ValueTask<Category?> SelectCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken);
    IQueryable<Category> SelectAllCategories();
}