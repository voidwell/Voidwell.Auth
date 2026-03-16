using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voidwell.Auth.Data;

public static class DatabaseExtensions
{
    public static IServiceCollection AddAuthData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AuthDB")
            ?? throw new InvalidOperationException("Connection string 'AuthDB' not found.");

        services.AddEntityFrameworkNpgsql();

        services.AddDbContext<AuthDbContext>(builder => AuthDbContextOptions.Configure(builder, connectionString));

        return services;
    }
}
