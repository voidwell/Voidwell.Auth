using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Voidwell.Common.Configuration;

public static class ServicePropertiesExtensions
{
    public static IServiceCollection ConfigureServiceProperties(this IServiceCollection services, string serviceName)
    {
        services.AddOptions();
        services.Configure<ServiceProperties>(config => config.Name = serviceName);
        services.AddTransient(a => a.GetRequiredService<IOptions<ServiceProperties>>().Value);

        return services;
    }
}
