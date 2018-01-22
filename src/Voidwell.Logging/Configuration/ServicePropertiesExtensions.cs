using Voidwell.Common.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServicePropertiesExtensions
    {
        public static void ConfigureServiceProperties(this IServiceCollection services, string serviceName)
        {
            services.AddOptions();
            services.Configure<ServiceProperties>(config => config.Name = serviceName);
            services.AddTransient(a => a.GetRequiredService<IOptions<ServiceProperties>>().Value);
        }
    }
}
