using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Voidwell.Auth.Services;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;

        public ResourceOwnerPasswordValidator(SignInManager<ApplicationUser> signInManager,
            IUserAuthenticationService userAuthenticationService, IHttpContextAccessor httpContextAccessor,
            ILogger<ResourceOwnerPasswordValidator> logger)
        {
            _signInManager = signInManager;
            _userAuthenticationService = userAuthenticationService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var httpContext = _httpContextAccessor?.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException($"Attempted to validate with null {nameof(HttpContext)}");
            }

            _httpContextAccessor.HttpContext.Items.TryGetValue(JwtClaimTypes.ClientId, out object clientId);
            if (clientId != null)
            {
                _httpContextAccessor.HttpContext.Items.Add(JwtClaimTypes.ClientId, context.Request.Client.ClientId);
            }

            var authRequest = new AuthenticationRequest
            {
                ClientId = context.Request.Client.ClientId,
                Username = context.UserName,
                Password = context.Password,
                UserAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown",
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString()
            };

            await _userAuthenticationService.Authenticate(authRequest);

            var claims = httpContext.User.Claims;
            var userId = claims.SingleOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;

            context.Result = new GrantValidationResult(userId, "password", claims);
        }
    }
}
