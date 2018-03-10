using System;
using System.Threading.Tasks;

namespace Voidwell.Common.Cache
{
    public interface ICache
    {
        Task SetAsync(string key, object value, TimeSpan? expires = null);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}