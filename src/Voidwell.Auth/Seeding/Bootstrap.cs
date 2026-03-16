using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voidwell.Auth.Seeding;

public static class SeedingBootstrap
{
    public static IServiceCollection AddSeeding(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SeedingConfig>(configuration.GetSection("Seeding"));
        services.AddHostedService<SeedingService>();
        
        return services;
    }
}