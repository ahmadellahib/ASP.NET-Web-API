using Microsoft.Extensions.Caching.Memory;

namespace CourseLibrary.API.Brokers.Caches;

public partial class CacheBroker : ICacheBroker
{
    private readonly IMemoryCache _memoryCache;

    public CacheBroker(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T GetCache<T>(string key)
    {
        return (T)_memoryCache.Get(key);
    }

    public void SetCache<T>(string key, T value)
    {
        MemoryCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        };

        SetCache(key, value, options);
    }

    public void SetCache<T>(string key, T value, MemoryCacheEntryOptions options)
    {
        _memoryCache.Set(key, value, options);
    }

    public void ClearCache(string key)
    {
        _memoryCache.Remove(key);
    }
}