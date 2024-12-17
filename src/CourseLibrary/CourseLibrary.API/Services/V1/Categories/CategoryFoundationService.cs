using CourseLibrary.API.Brokers.Caches;
using CourseLibrary.API.Brokers.Logging;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Validators.Categories;

namespace CourseLibrary.API.Services.V1.Categories;

internal sealed class CategoryFoundationService : ICategoryFoundationService
{
    private readonly ICacheBroker _cacheBroker;
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<CategoryFoundationService> _loggingBroker;

    public CategoryFoundationService(ICacheBroker cacheBroker,
        IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<CategoryFoundationService> loggingBroker)
    {
        _cacheBroker = cacheBroker ?? throw new ArgumentNullException(nameof(cacheBroker));
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
    }

    public async Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(category, new CategoryValidator(true));
        _cacheBroker.ClearCachedCategories();

        return await _storageBroker.InsertCategoryAsync(category, cancellationToken);
    }

    public async Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(category, new CategoryValidator(false));
        _cacheBroker.ClearCachedCategories();

        return await _storageBroker.UpdateCategoryAsync(category, cancellationToken);
    }

    public Category RetrieveCategoryById(Guid categoryId)
    {
        _servicesLogicValidator.ValidateParameter(categoryId, nameof(categoryId));

        List<Category>? storageCategories = _cacheBroker.GetCachedCategories();

        if (storageCategories is null)
        {
            storageCategories = _storageBroker.SelectAllCategories().ToList();
            _cacheBroker.SetCachedCategories(storageCategories);
        }

        Category? storageCategory = storageCategories.FirstOrDefault(category => category.Id == categoryId);

        _servicesLogicValidator.ValidateStorageEntity<Category>(storageCategory, categoryId);

        return storageCategory!;
    }

    public IEnumerable<Category> RetrieveAllCategories()
    {
        List<Category>? storageCategories = _cacheBroker.GetCachedCategories();

        if (storageCategories is null)
        {
            storageCategories = _storageBroker.SelectAllCategories().ToList();
            _cacheBroker.SetCachedCategories(storageCategories);
        }

        if (storageCategories.Count == 0)
        {
            _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
        }

        return storageCategories;
    }
}