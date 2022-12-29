using CourseLibrary.API.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CourseLibrary.API.Brokers.Storages;

internal sealed partial class StorageBroker
{
    internal DbSet<Category> Categories { get; set; }

    public async Task<Category> InsertCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        EntityEntry<Category> categoryEntityEntry = await Categories.AddAsync(category, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return categoryEntityEntry.Entity;
    }

    public async Task<Category> UpdateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        EntityEntry<Category> categoryEntityEntry = Categories.Update(category);
        await SaveChangesAsync(cancellationToken);

        return categoryEntityEntry.Entity;
    }

    public async ValueTask<Category?> SelectCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken) =>
         await Categories.FindAsync(new object[] { categoryId }, cancellationToken);

    public IQueryable<Category> SelectAllCategories() =>
        Categories.AsQueryable();
}
