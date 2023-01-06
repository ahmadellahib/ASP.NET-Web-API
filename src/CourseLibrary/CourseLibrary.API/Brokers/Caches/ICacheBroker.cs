using Microsoft.Extensions.Caching.Memory;

namespace CourseLibrary.API.Brokers.Caches;

public partial interface ICacheBroker
{
    T GetCache<T>(string key);

    void SetCache<T>(string key, T value);

    void SetCache<T>(string key, T value, MemoryCacheEntryOptions options);

    void ClearCache(string key);
}