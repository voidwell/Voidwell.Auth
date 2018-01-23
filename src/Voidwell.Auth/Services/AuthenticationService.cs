using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Voidwell.Auth.Clients;
using Voidwell.Auth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Voidwell.Auth.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserManagementClient _userManagementClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public AuthenticationService(IUserManagementClient userManagementClient, IHttpContextAccessor httpContextAccessor, ILogger<AuthenticationService> logger)
        {
            _userManagementClient = userManagementClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> Authenticate(AuthenticationRequest authRequest)
        {
            var authResult = await _userManagementClient.Authenticate(authRequest);
            if (authResult == null)
                return null;

            _logger.LogDebug($"Got authentication result for user {authResult?.UserId.ToString()}");

            if (authResult.Error != null)
            {
                return authResult.Error;
            }

            var name = authResult.Claims.FirstOrDefault(a => a.Type == JwtClaimTypes.Name)?.Value;

            var user = new IdentityServerUser(authResult.UserId.ToString())
            {
                DisplayName = name,
                AuthenticationTime = DateTime.UtcNow,
                AdditionalClaims = authResult.Claims.Where(AdditionalClaimFilter).ToArray(),
                IdentityProvider = "auth.localdev.com"
            };
            var principal = user.CreatePrincipal();

            var authProps = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await _httpContextAccessor.HttpContext.SignInAsync("voidwell", principal, authProps);
            return null;
        }

        private static bool AdditionalClaimFilter(Claim claim)
        {
            return claim.Type != JwtClaimTypes.Name
                && claim.Type != JwtClaimTypes.Subject;
        }
    }
}
