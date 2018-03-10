using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Voidwell.Auth.Clients;

namespace Voidwell.VoidwellAuth.Client
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly IUserManagementClient _userManagementClient;

        public ClaimsTransformer(IUserManagementClient userManagementClient)
        {
            _userManagementClient = userManagementClient;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                IEnumerable<string> roles;
                try
                {
                    var sub = principal.FindFirstValue(JwtClaimTypes.Subject);
                    roles = await _userManagementClient.GetRoles(sub);
                }
                catch (Exception)
                {
                    return principal;
                }

                if (roles != null && roles.Any())
                {
                    var claims = roles.Select(role => new Claim(JwtClaimTypes.Role, role));

                    var id = ((ClaimsIdentity)principal.Identity);
                    id.AddClaims(claims);
                }
            }

            return principal;
        }
    }
}
