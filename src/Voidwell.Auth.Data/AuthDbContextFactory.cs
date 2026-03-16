using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Voidwell.Auth.Data;

internal class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString();

        var builder = new DbContextOptionsBuilder<AuthDbContext>();
        AuthDbContextOptions.Configure(builder, connectionString);

        return new AuthDbContext(builder.Options);
    }

    private static string GetConnectionString()
    {
        var configPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "Voidwell.Auth")
        );

        var config = new ConfigurationBuilder()
            .SetBasePath(configPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return config.GetConnectionString("AuthDB")
            ?? throw new InvalidOperationException("Connection string 'AuthDB' not found.");
    }
}
