using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.Common.Configuration;
using ZiggyCreatures.Caching.Fusion;

namespace Voidwell.Common.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheOptions = configuration.Get<CacheOptions>();

        var cacheBuilder = services
        .AddFusionCache()
        .WithOptions(options =>
        {
            options.CacheKeyPrefix = typeof(CacheExtensions).Assembly.GetName().Name;
        });

        if (!string.IsNullOrWhiteSpace(cacheOptions.RedisConfiguration))
        {
            cacheBuilder.WithStackExchangeRedisBackplane(options =>
            {
                options.Configuration = cacheOptions.RedisConfiguration;
            });
        }

        services.AddSingleton<ICache, CacheWrapper>();

        return services;
    }
}
