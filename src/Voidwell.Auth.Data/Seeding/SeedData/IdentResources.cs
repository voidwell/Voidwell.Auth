using System.Collections.Generic;
using IdentityServer4.Models;

namespace Voidwell.Auth.Data.Seeding.SeedData
{
    public static class IdentResources
    {
        public static IEnumerable<IdentityResource> GetSeeds()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "voidwell-roles",
                    UserClaims = new[]
                    {
                        "role"
                    }
                }
            };
        }
    }
}
