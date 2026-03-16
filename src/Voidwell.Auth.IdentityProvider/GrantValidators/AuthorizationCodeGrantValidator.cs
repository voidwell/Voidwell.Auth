using System;
using System.Collections.Generic;
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

internal class AuthorizationCodeGrantValidator : IGrantValidator
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthorizationCodeGrantValidator(
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        IOpenIddictScopeManager scopeManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
        _scopeManager = scopeManager;
    }

    public string GrantType => GrantTypes.AuthorizationCode;

    public async Task<GrantValidationResult> ValidateAsync(OpenIddictRequest request)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var result = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (result?.Principal == null)
        {
            return new GrantValidationResult(Errors.InvalidGrant);
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

        var userRoles = await _userService.GetRoles(userId);
        var scopes = result.Principal.GetScopes();
        var resources = await _scopeManager.ListResourcesAsync(scopes).ToListAsync();

        var claims = new List<Claim>
        {
            new(Claims.Subject, user.Id.ToString()),
            new(Claims.Email, user.Email ?? string.Empty),
            new(Claims.Name, user.UserName ?? string.Empty)
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(Claims.Role, role));
        }

        foreach (var scope in scopes)
        {
            claims.Add(new Claim(Claims.Private.Scope, scope));
        }

        foreach (var resource in resources)
        {
            claims.Add(new Claim(Claims.Private.Audience, resource));
        }

        var authorizationId = result.Principal.GetAuthorizationId();
        if (!string.IsNullOrEmpty(authorizationId))
        {
            claims.Add(new Claim(Claims.Private.AuthorizationId, authorizationId));
        }

        return new GrantValidationResult(user.Id.ToString(), GrantTypes.AuthorizationCode, claims);
    }
}
