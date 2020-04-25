using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.Auth.Data.Seeding
{
    public static class Seeder
    {
        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app, IConfiguration configuration)
        {
            var seedingOptions = configuration.Get<SeedingOptions>();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();

                var dbClients = dbContext.Clients.Include(a => a.AllowedScopes).Include(a => a.ClientSecrets).Include(a => a.AllowedGrantTypes).ToList();
                var dbApiResources = dbContext.ApiResources.Include(a => a.Scopes).Include(a => a.Secrets).ToList();
                var dbIdentityResources = dbContext.IdentityResources;

                foreach (var client in SeedData.Clients.GetSeeds(seedingOptions).Select(a => a.ToEntity()))
                {
                    if (!dbClients.Any(a => a.ClientId == client.ClientId))
                    {
                        dbContext.Clients.Add(client);
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

                foreach (var apiResource in SeedData.ApiResources.GetSeeds(seedingOptions).Select(a => a.ToEntity()))
                {
                    if (!dbApiResources.Any(a => a.Name == apiResource.Name))
                    {
                        dbContext.ApiResources.Add(apiResource);
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
                        }
                        else if (dbResource.Scopes != null && dbResource.Scopes.Any())
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

                foreach (var identityResource in SeedData.IdentResources.GetSeeds().Select(a => a.ToEntity()))
                {
                    if (!dbIdentityResources.Any(a => a.Name == identityResource.Name))
                    {
                        dbContext.IdentityResources.Add(identityResource);
                    }
                }

                dbContext.SaveChanges();
            }

            return app;
        }
    }
}
