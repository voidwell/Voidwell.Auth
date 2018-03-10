using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Voidwell.Common.Configuration;

namespace Voidwell.Common.Cache
{
    public class CacheWrapper : ICache, IDisposable
    {
        private CacheOptions _options;
        private ConnectionMultiplexer _redis;
        private IDatabase _db;

        private readonly string _keyPrefix;

        public CacheWrapper(CacheOptions options, ServiceProperties serviceProperties)
        {
            _options = options;
            _keyPrefix = serviceProperties?.Name;

            Task.Run(() => Connect());
        }

        public Task SetAsync(string key, object value, TimeSpan? expires = null)
        {
            try
            {
                var sValue = JsonConvert.SerializeObject(value);
                return _db.StringSetAsync(KeyFormatter(key), sValue, expiry: expires);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(KeyFormatter(key));

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                return _db.KeyDeleteAsync(KeyFormatter(key));
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }

        private async Task Connect()
        {
            _redis = await ConnectionMultiplexer.ConnectAsync(_options.RedisConfiguration);
            _db = _redis.GetDatabase();
        }

        private string KeyFormatter(string key)
        {
            return $"{_keyPrefix}_{key}";
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
