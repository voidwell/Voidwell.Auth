using System;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace Voidwell.Common.Cache;

public class CacheWrapper : ICache
{
    private readonly IFusionCache _cache;

    public CacheWrapper(IFusionCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync(string key, object value, TimeSpan expires)
    {
        try
        {
            await _cache.SetAsync(key, value, expires);
        }
        catch (Exception)
        {
            return;
        }
    }

    public async Task<T> GetAsync<T>(string key)
    {
        try
        {
            return await _cache.GetOrDefaultAsync<T>(key);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception)
        {
            return;
        }
    }
}
