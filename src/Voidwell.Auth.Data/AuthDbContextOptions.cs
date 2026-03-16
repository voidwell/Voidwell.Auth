using Microsoft.EntityFrameworkCore;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Data;

internal static class AuthDbContextOptions
{
    public static void Configure(DbContextOptionsBuilder builder, string connectionString)
    {
        builder.UseNpgsql(
            connectionString,
            b => b.MigrationsAssembly(typeof(AuthDbContext).Assembly)
        );

        builder.UseOpenIddict<
            AuthApplication,
            AuthAuthorization,
            AuthScope,
            AuthToken,
            int>();
    }
}
