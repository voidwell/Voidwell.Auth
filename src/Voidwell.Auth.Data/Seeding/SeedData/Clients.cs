using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Voidwell.Auth.Data.Seeding.SeedData
{
    public static class Clients
    {
        public static IEnumerable<Client> GetSeeds(SeedingOptions options)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "voidwell-clientui",
                    ClientName = "Voidwell Client UI",
                    AllowedGrantTypes = { GrantType.Implicit },
                    RequireConsent = false,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "voidwell-api"
                    },
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = 21600,
                    AllowAccessTokensViaBrowser = true,
                    EnableLocalLogin = true,
                    IdentityTokenLifetime = 21600,
                    PostLogoutRedirectUris =
                    {
                        "https://voidwell.com/",
                        "http://voidwellclientui:5000/",
                        "http://localdev.com/",
                    },
                    RedirectUris =
                    {
                        "http://voidwellclientui:5000/signInCallback.html",
                        "http://voidwellclientui:5000/",
                        "https://voidwell.com/",
                        "https://voidwell.com/signInCallback.html",
                        "https://voidwell.com/silentCallback.html",
                        "http://localdev.com/",
                        "http://localdev.com/signInCallback.html",
                        "http://localdev.com/silentCallback.html"
                    },
                    AllowedCorsOrigins =
                    {
                        "https://voidwell.com",
                        "http://voidwellclientui:5000",
                        "http://localdev.com"
                    }
                },
                new Client
                {
                    ClientId = "voidwell-auth",
                    ClientName = "Voidwell Auth",
                    AllowedGrantTypes = new List<string> { GrantType.ClientCredentials },

                    ClientSecrets =
                    {
                        new Secret(options.AuthClientSecret.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "voidwell-usermanagement"
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 21600
                },
                new Client
                {
                    ClientId = "voidwell-api",
                    ClientName = "Voidwell API",
                    AllowedGrantTypes = new List<string> { GrantType.ClientCredentials, "delegation" },

                    ClientSecrets =
                    {
                        new Secret(options.ApiClientSecret.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "voidwell-daybreakgames",
                        "voidwell-bungienet",
                        "voidwell-usermanagement",
                        "voidwell-internal",
                        "voidwell-auth-admin"
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 21600
                },
                new Client
                {
                    ClientId = "voidwell-usermanagement",
                    ClientName = "Voidwell User Management",
                    AllowedGrantTypes = new List<string> { GrantType.ClientCredentials },

                    ClientSecrets =
                    {
                        new Secret(options.UserManagementClientSecret.Sha256())
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 21600
                },
                new Client
                {
                    ClientId = "voidwell-internal",
                    ClientName = "Voidwell Internal",
                    AllowedGrantTypes = new List<string> { GrantType.ClientCredentials },

                    ClientSecrets =
                    {
                        new Secret(options.InternalClientSecret.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "voidwell-daybreakgames",
                        "voidwell-bungienet",
                        "voidwell-usermanagement"
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 21600
                },
                new Client
                {
                    ClientId = "voidwell-bungienet",
                    ClientName = "Voidwell Bungie.Net",
                    AllowedGrantTypes = new List<string> { GrantType.ClientCredentials },

                    ClientSecrets =
                    {
                        new Secret(options.BungieNetClientSecret.Sha256())
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 21600
                },
                new Client
                {
                    ClientId = "voidwell-daybreakgames",
                    ClientName = "Voidwel Daybreak Games",
                    AllowedGrantTypes = new List<string> { GrantType.ClientCredentials },

                    ClientSecrets =
                    {
                        new Secret(options.DaybreakGamesClientSecret.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "voidwell-messagewell-publish"
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 21600
                },
                new Client
                {
                    ClientId = "mutterblack",
                    ClientName = "Mutterblack",
                    AllowedGrantTypes = new List<string> { GrantType.ClientCredentials },

                    ClientSecrets =
                    {
                        new Secret(options.MutterblackClientSecret.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "voidwell-daybreakgames",
                        "voidwell-api"
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 21600
                },
                new Client
                {
                    ClientId = "voidwell-filewell",
                    ClientName = "Voidwell FileWell UI",
                    AllowedGrantTypes = { GrantType.Hybrid },
                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret(options.FilewellClientSecret.Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "voidwell-roles"
                    },
                    AllowOfflineAccess = true,
                    PostLogoutRedirectUris =
                    {
                        "http://f.localdev.com/signout-callback-oidc",
                        "http://localhost:5000/signout-callback-oidc",
                        "https://f.voidwell.com/signout-callback-oidc"
                    },
                    RedirectUris =
                    {
                        "https://f.voidwell.com/signin-oidc",
                        "http://f.localdev.com/signin-oidc",
                        "http://localhost:5000/signin-oidc"
                    },
                    AllowedCorsOrigins =
                    {
                        "https://voidwell.com",
                        "https://f.voidwell.com",
                        "http://voidwellfilewell:5000",
                        "http://localdev.com",
                        "http://f.localdev.com",
                        "http://localhost",
                    }
                }
            };
        }
    }
}
