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
using System.Collections.Generic;

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

            return services;
        }

        public static IIdentityServerBuilder AddIdentityServerStores(this IIdentityServerBuilder idsvBuilder, IConfiguration configuration)
        {
            var dbOptions = configuration.Get<DatabaseOptions>();

            idsvBuilder.Services.AddEntityFrameworkNpgsql();

            idsvBuilder.AddConfigurationStore(options =>
                options.ConfigureDbContext = builder =>
                    builder.UseNpgsql(dbOptions.DBConnectionString, b =>
                    {
                        b.MigrationsAssembly(_migrationAssembly);
                        b.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null);
                    }));

            idsvBuilder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseNpgsql(dbOptions.DBConnectionString, b =>
                    {
                        b.MigrationsAssembly(_migrationAssembly);
                        b.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null);
                    });

                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            });

            return idsvBuilder;
        }

        public static IApplicationBuilder InitializeDatabases(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                List<DbContext> dbContextList = new List<DbContext>();
                var sp = serviceScope.ServiceProvider;

                dbContextList.Add(sp.GetRequiredService<PersistedGrantDbContext>());
                dbContextList.Add(sp.GetRequiredService<ConfigurationDbContext>());

                dbContextList.ForEach(a => a.Database.Migrate());
            }

            return app;
        }

        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                var dbClients = dbContext.Clients.ToList();
                var dbApiResources = dbContext.ApiResources.ToList();
                var dbIdentityResources = dbContext.IdentityResources;

                foreach (var client in Seeding.Clients.GetSeeds().Select(a => a.ToEntity()))
                {
                    if (!dbClients.Any(a => a.ClientId == client.ClientId))
                    {
                        dbContext.Add(client);
                    }
                }

                foreach (var apiResource in Seeding.ApiResources.GetSeeds().Select(a => a.ToEntity()))
                {
                    if (!dbApiResources.Any(a => a.Name == apiResource.Name))
                    {
                        dbContext.Add(apiResource);
                    }
                }

                foreach (var identityResource in Seeding.IdentResources.GetSeeds().Select(a => a.ToEntity()))
                {
                    if (!dbIdentityResources.Any(a => a.Name == identityResource.Name))
                    {
                        dbContext.Add(identityResource);
                    }
                }

                dbContext.SaveChanges();
            }

            return app;
        }
    }
}
