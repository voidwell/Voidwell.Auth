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
using IdentityServer4.Models;
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
                    builder.UseNpgsql(dbOptions.DBConnectionString, b => b.MigrationsAssembly(_migrationAssembly)));

            idsvBuilder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseNpgsql(dbOptions.DBConnectionString, b => b.MigrationsAssembly(_migrationAssembly));

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
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    var clients = Seeding.Clients.GetClients().Select(a => a.ToEntity());
                    context.Clients.AddRange(clients);
                }

                if (!context.ApiResources.Any())
                {
                    var apiResources = Seeding.ApiResources.GetApiResources().Select(a => a.ToEntity());
                    context.ApiResources.AddRange(apiResources);
                }

                if (!context.IdentityResources.Any())
                {
                    context.IdentityResources.Add(new IdentityResources.OpenId().ToEntity());
                    context.IdentityResources.Add(new IdentityResources.Profile().ToEntity());
                    context.IdentityResources.Add(new IdentityResources.Email().ToEntity());
                }

                context.SaveChanges();
            }

            return app;
        }
    }
}
