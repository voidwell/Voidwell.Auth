using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IUserAuthenticationService _userAuthenticationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResourceOwnerPasswordValidator(IUserAuthenticationService userAuthenticationService, IHttpContextAccessor httpContextAccessor)
    {
        _userAuthenticationService = userAuthenticationService;
        _httpContextAccessor = httpContextAccessor;
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
            UserAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown",
            IpAddress = httpContext.Connection.RemoteIpAddress?.ToString()
        };

        await _userAuthenticationService.Authenticate(authRequest);

        var claims = httpContext.User.Claims;
        var userId = claims.SingleOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;

        context.Result = new GrantValidationResult(userId, "password", claims);
    }
}
