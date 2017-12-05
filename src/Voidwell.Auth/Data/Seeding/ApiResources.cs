using System.Collections.Generic;
using IdentityServer4.Models;

namespace Voidwell.Auth.Data.Seeding
{
    public static class ApiResources
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "voidwell-api",
                    DisplayName = "Voidwell Api",
                    ApiSecrets =
                    {
                        new Secret("adminApiResourceSecret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "voidwell-api",
                            DisplayName = "Voidwell API"
                        }
                    }
                },
                new ApiResource
                {
                    Name = "voidwell-usermanagement",
                    DisplayName = "Voidwell User Management",
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "voidwell-usermanagement",
                            DisplayName = "Voidwell User Management"
                        }
                    }
                },
                new ApiResource
                {
                    Name = "voidwell-internal",
                    DisplayName = "Voidwell Internal",
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "voidwell-internal",
                            DisplayName = "Voidwell Internal"
                        }
                    }
                },
                new ApiResource
                {
                    Name = "voidwell-daybreakgames",
                    DisplayName = "Voidwell Daybreak Games",
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "voidwell-daybreakgames",
                            DisplayName = "Voidwell Daybreak Games"
                        }
                    }
                },
                new ApiResource
                {
                    Name = "voidwell-bungienet",
                    DisplayName = "Voidwell Bungie.Net",
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "voidwell-bungienet",
                            DisplayName = "Voidwell Bungie.Net"
                        }
                    }
                }
            };
        }
    }
}
