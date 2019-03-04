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
using IdentityServer4.EntityFramework.Entities;

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

                var dbClients = dbContext.Clients.Include(a => a.AllowedScopes).Include(a => a.ClientSecrets).Include(a => a.AllowedGrantTypes).ToList();
                var dbApiResources = dbContext.ApiResources.Include(a => a.Scopes).Include(a => a.Secrets).ToList();
                var dbIdentityResources = dbContext.IdentityResources;

                foreach (var client in Seeding.Clients.GetSeeds().Select(a => a.ToEntity()))
                {
                    if (!dbClients.Any(a => a.ClientId == client.ClientId))
                    {
                        dbContext.Add(client);
                    }
                    else
                    {
                        var dbClient = dbClients.FirstOrDefault(a => a.ClientId == client.ClientId);

                        if (client.AllowedScopes.Any())
                        {
                            if (dbClient.AllowedScopes == null)
                            {
                                dbClient.AllowedScopes = new List<ClientScope>();
                            }
                            var newScopes = client.AllowedScopes.Where(a => !dbClient.AllowedScopes.Any(b => b.Scope == a.Scope));
                            dbClient.AllowedScopes.AddRange(newScopes);
                            dbClient.AllowedScopes.RemoveAll(a => !client.AllowedScopes.Any(b => b.Scope == a.Scope));
                        }
                        else if (dbClient.AllowedScopes != null && dbClient.AllowedScopes.Any())
                        {
                            dbClient.AllowedScopes.Clear();
                        }

                        if (client.AllowedGrantTypes.Any())
                        {
                            if (dbClient.AllowedGrantTypes == null)
                            {
                                dbClient.AllowedGrantTypes = new List<ClientGrantType>();
                            }
                            var newGrantTypes = client.AllowedGrantTypes.Where(a => !dbClient.AllowedGrantTypes.Any(b => b.GrantType == a.GrantType));
                            dbClient.AllowedGrantTypes.AddRange(newGrantTypes);
                            dbClient.AllowedGrantTypes.RemoveAll(a => !client.AllowedGrantTypes.Any(b => b.GrantType == a.GrantType));
                        }
                        else if (dbClient.AllowedGrantTypes != null && dbClient.AllowedGrantTypes.Any())
                        {
                            dbClient.AllowedGrantTypes.Clear();
                        }

                        if (client.ClientSecrets.Any())
                        {
                            if (dbClient.ClientSecrets == null)
                            {
                                dbClient.ClientSecrets = new List<ClientSecret>();
                            }
                            var newClientSecrets = client.ClientSecrets.Where(a => !dbClient.ClientSecrets.Any(b => b.Value == a.Value));
                            dbClient.ClientSecrets.AddRange(newClientSecrets);
                            dbClient.ClientSecrets.RemoveAll(a => !client.ClientSecrets.Any(b => b.Value == a.Value));
                        }
                        else if (dbClient.ClientSecrets != null && dbClient.ClientSecrets.Any())
                        {
                            dbClient.ClientSecrets.Clear();
                        }

                        dbClient.AccessTokenLifetime = client.AccessTokenLifetime;
                        dbClient.RequireClientSecret = client.RequireClientSecret;
                    }
                }

                foreach (var apiResource in Seeding.ApiResources.GetSeeds().Select(a => a.ToEntity()))
                {
                    if (!dbApiResources.Any(a => a.Name == apiResource.Name))
                    {
                        dbContext.Add(apiResource);
                    }
                    else
                    {
                        var dbResource = dbApiResources.FirstOrDefault(a => a.Name == apiResource.Name);
                        
                        if (apiResource.Scopes.Any())
                        {
                            if (dbResource.Scopes == null)
                            {
                                dbResource.Scopes = new List<ApiScope>();
                            }

                            var original = dbResource.Scopes.Select(a => $"{a.Id} {a.Name}");

                            var newScopes = apiResource.Scopes.Where(a => !dbResource.Scopes.Any(b => b.Name == a.Name));
                            dbResource.Scopes.AddRange(newScopes);
                            dbResource.Scopes.RemoveAll(a => !apiResource.Scopes.Any(b => b.Name == a.Name));

                            var modified = dbResource.Scopes.Select(a => $"{a.Id} {a.Name}");
                        } else if (dbResource.Scopes != null && dbResource.Scopes.Any())
                        {
                            dbResource.Scopes.Clear();
                        }

                        if (apiResource.Secrets.Any())
                        {
                            if (dbResource.Secrets == null)
                            {
                                dbResource.Secrets = new List<ApiSecret>();
                            }
                            var newClientSecrets = apiResource.Secrets.Where(a => !dbResource.Secrets.Any(b => b.Value == a.Value));
                            dbResource.Secrets.AddRange(newClientSecrets);
                            dbResource.Secrets.RemoveAll(a => !apiResource.Secrets.Any(b => b.Value == a.Value));
                        }
                        else if (dbResource.Secrets != null && dbResource.Secrets.Any())
                        {
                            dbResource.Secrets.Clear();
                        }

                        dbResource.Secrets = apiResource.Secrets;
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
