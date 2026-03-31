using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.IdentityProvider.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.IdentityProvider.GrantValidators;

internal class PasswordGrantValidator : IGrantValidator
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserService _userService;
    private readonly IOpenIddictScopeManager _scopeManager;

    public PasswordGrantValidator(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserService userService,
        IOpenIddictScopeManager scopeManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
        _scopeManager = scopeManager;
    }

    public string GrantType => GrantTypes.Password;

    public async Task<GrantValidationResult> ValidateAsync(OpenIddictRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return new GrantValidationResult(Errors.InvalidGrant);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return new GrantValidationResult(Errors.InvalidGrant);
        }

        var userRoles = await _userService.GetRoles(user.Id);
        var scopes = request.GetScopes();
        var resources = await _scopeManager.ListResourcesAsync(scopes).ToListAsync();

        var claims = new List<Claim>
        {
            new(Claims.Subject, user.Id.ToString()),
            new(Claims.Email, user.Email ?? string.Empty),
            new(Claims.EmailVerified, user.EmailConfirmed.ToString().ToLowerInvariant()),
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

        return new GrantValidationResult(user.Id.ToString(), GrantTypes.Password, claims);
    }
}
