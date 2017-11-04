using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.Linq;

namespace Voidwell.Auth.Data
{
    public static class DatabaseExtensions
    {
        private static string _migrationAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<DatabaseOptions>>().Value);
            services.Configure<DatabaseOptions>(configuration);

            var options = configuration.Get<DatabaseOptions>();

            services.AddEntityFrameworkNpgsql();
            services.AddDbContext<AuthDbContext>(builder =>
                builder.UseNpgsql(options.DBConnectionString, b => b.MigrationsAssembly(_migrationAssembly)));
            services.AddTransient(sp => new Func<AuthDbContext>(() => sp.GetRequiredService<AuthDbContext>()));

            return services;
        }

        public static IIdentityServerBuilder AddIdentityServerStores(this IIdentityServerBuilder idsvBuilder, IConfiguration configuration)
        {
            var dbOptions = configuration.Get<DatabaseOptions>();

            idsvBuilder.Services.AddEntityFrameworkNpgsql();

            idsvBuilder.AddConfigurationStore(options =>
                options.ConfigureDbContext = builder =>
                    builder.UseNpgsql(dbOptions.DBConnectionString, b => b.MigrationsAssembly(_migrationAssembly)));

            idsvBuilder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseNpgsql(dbOptions.DBConnectionString, b => b.MigrationsAssembly(_migrationAssembly));

                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });

            return idsvBuilder;
        }

        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<AuthDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                /*
                if (!context.Clients.Any())
                {
                    foreach (var client in SeedConfig.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in SeedConfig.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in SeedConfig.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                */
            }

            return app;
        }
    }
}
