using CourseLibrary.API.Models.Categories;
using Microsoft.Extensions.Caching.Memory;

namespace CourseLibrary.API.Brokers.Caches;

public partial class CacheBroker
{
    private const string CategoriesCacheKey = "CategoriesCacheKey";

    public List<Category>? GetCachedCategories()
    {
        return GetCache<List<Category>?>(CategoriesCacheKey);
    }

    public void SetCachedCategories(List<Category> categoriesList)
    {
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(24));

        SetCache(CategoriesCacheKey, categoriesList, cacheEntryOptions);
    }

    public void ClearCachedCategories()
    {
        ClearCache(CategoriesCacheKey);
    }
}