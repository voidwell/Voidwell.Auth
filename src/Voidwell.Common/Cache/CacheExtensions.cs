using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voidwell.Common.Cache
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<CacheOptions>(configuration);

            services.AddSingleton<ICache, CacheWrapper>();

            return services;
        }
    }
}
