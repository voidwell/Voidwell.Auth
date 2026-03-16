using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Voidwell.Auth.IdentityProvider.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.IdentityProvider.GrantValidators;

internal class DeviceCodeGrantValidator : IGrantValidator
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public DeviceCodeGrantValidator(
        IHttpContextAccessor httpContextAccessor,
        IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public string GrantType => GrantTypes.DeviceCode;

    public async Task<GrantValidationResult> ValidateAsync(OpenIddictRequest request)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (result?.Principal == null)
        {
            return new GrantValidationResult(Errors.AuthorizationPending);
        }

        var subjectClaim = result.Principal.GetClaim(Claims.Subject);
        if (string.IsNullOrEmpty(subjectClaim) || !Guid.TryParse(subjectClaim, out var userId))
        {
            return new GrantValidationResult(Errors.InvalidGrant);
        }

        var user = await _userService.GetUser(userId);
        if (user == null)
        {
            return new GrantValidationResult(Errors.InvalidGrant);
        }

        // Copy existing claims from the principal
        var claims = result.Principal.Claims.ToList();

        // Ensure subject claim exists
        if (!claims.Any(c => c.Type == Claims.Subject))
        {
            claims.Insert(0, new Claim(Claims.Subject, user.Id.ToString()));
        }

        return new GrantValidationResult(user.Id.ToString(), GrantTypes.DeviceCode, claims);
    }
}
