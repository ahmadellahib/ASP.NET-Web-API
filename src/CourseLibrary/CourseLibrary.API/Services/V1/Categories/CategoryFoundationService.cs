using CourseLibrary.API;
using CourseLibrary.API.Brokers.Caches;
using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Services;
using CourseLibrary.API.Validators.Categories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CategoryLibrary.API.Services.V1.Categories;

internal sealed class CategoryFoundationService : ICategoryFoundationService
{
    private readonly ICacheBroker _cacheBroker;
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<CategoryFoundationService> _loggingBroker;
    private readonly IServicesExceptionsLogger<CategoryFoundationService> _servicesExceptionsLogger;

    public CategoryFoundationService(ICacheBroker cacheBroker,
        IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<CategoryFoundationService> loggingBroker,
        IServicesExceptionsLogger<CategoryFoundationService> servicesExceptionsLogger)
    {
        _cacheBroker = cacheBroker ?? throw new ArgumentNullException(nameof(cacheBroker));
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(category, new CategoryValidator(true));

            _cacheBroker.ClearCachedCategories();

            return await _storageBroker.InsertCategoryAsync(category, cancellationToken);
        }
        catch (InvalidEntityException<Category> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<Category> lockedEntityException = new(dbUpdateConcurrencyException);

            throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            throw _servicesExceptionsLogger.CreateAndLogDependencyException(dbUpdateException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(category, new CategoryValidator(false));

            _cacheBroker.ClearCachedCategories();

            return await _storageBroker.UpdateCategoryAsync(category, cancellationToken);
        }
        catch (InvalidEntityException<Category> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<Category> lockedEntityException = new(dbUpdateConcurrencyException);

            throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            throw _servicesExceptionsLogger.CreateAndLogDependencyException(dbUpdateException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public Category RetrieveCategoryById(Guid categoryId)
    {
        try
        {
            _servicesLogicValidator.ValidateParameter(categoryId, nameof(categoryId));

            Category? storageCategory;
            List<Category> storageCategories = _cacheBroker.GetCachedCategories();

            if (storageCategories is null)
            {
                storageCategories = _storageBroker.SelectAllCategories().ToList();
                _cacheBroker.SetCachedCategories(storageCategories);
            }

            storageCategory = storageCategories.FirstOrDefault(category => category.Id == categoryId);

            _servicesLogicValidator.ValidateStorageEntity<Category>(storageCategory, categoryId);

            return storageCategory!;
        }
        catch (InvalidParameterException invalidIdException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(invalidIdException);
        }
        catch (NotFoundEntityException<Category> notFoundEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(notFoundEntityException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public IEnumerable<Category> RetrieveAllCategories()
    {
        try
        {
            List<Category> storageCategories = _cacheBroker.GetCachedCategories();

            if (storageCategories is null)
            {
                storageCategories = _storageBroker.SelectAllCategories().ToList();
                _cacheBroker.SetCachedCategories(storageCategories);
            }

            if (!storageCategories.Any())
            {
                _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
            }

            return storageCategories;
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}
