using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using System.Collections.Generic;
using Voidwell.Auth.Data.Seeding;
using Voidwell.Auth.Data.Repositories;

namespace Voidwell.Auth.Data
{
    public static class DatabaseExtensions
    {
        private static string _migrationAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;
        private static object _initializeLock = new object();
        private static bool _initialized = false;

        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<DatabaseOptions>>().Value);
            services.Configure<DatabaseOptions>(configuration);

            services.AddEntityFrameworkNpgsql();

            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IApiResourceRepository, ApiResourceRepository>();

            return services;
        }

        public static IIdentityServerBuilder AddIdentityServerStores(this IIdentityServerBuilder idsvBuilder, IConfiguration configuration)
        {
            var dbOptions = configuration.Get<DatabaseOptions>();

            idsvBuilder.Services.AddEntityFrameworkNpgsql();

            idsvBuilder.AddConfigurationStore<IdentityServerConfigurationDbContext>(options =>
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

        public static void InitializeDatabases(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (_initialized)
            {
                return;
            }

            lock (_initializeLock)
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    List<DbContext> dbContextList = new List<DbContext>();
                    var sp = serviceScope.ServiceProvider;

                    dbContextList.Add(sp.GetRequiredService<PersistedGrantDbContext>());
                    dbContextList.Add(sp.GetRequiredService<IdentityServerConfigurationDbContext>());

                    dbContextList.ForEach(a => a.Database.Migrate());
                }

                app.SeedDatabase(configuration);

                _initialized = true;
            }
        }
    }
}
