using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;
using Voidwell.VoidwellAuth.Data.DBContext;

namespace Voidwell.VoidwellAuth.Data
{
    public static class IdentityServerDataExtensions
    {
        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services)
        {
            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(Configuration.ConnectionString));

            return services;
        }

        public static IIdentityServerBuilder AddIdentityServerStores(this IIdentityServerBuilder idsvBuilder)
        {
            idsvBuilder.AddConfigurationStore(builder =>
                builder.UseSqlServer(Configuration.ConnectionString, options =>
                    options.MigrationsAssembly(Configuration.MigrationsAssembly)));

            idsvBuilder.AddOperationalStore(builder =>
                builder.UseSqlServer(Configuration.ConnectionString, options =>
                    options.MigrationsAssembly(Configuration.MigrationsAssembly)));

            return idsvBuilder;
        }

        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<UserDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
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
            }

            return app;
        }
    }
}
