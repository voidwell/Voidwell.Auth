using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.UserManagement.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Controllers;

[Route("connect/userinfo")]
public class UserInfoController : ControllerBase
{
    private readonly IUserService _userService;

    public UserInfoController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet, HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public async Task<IActionResult> UserInfo()
    {
        // Get the user from the access token claims
        var userId = User.GetUserId();

        var user = await _userService.GetUser(userId);
        if (user is null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new Microsoft.AspNetCore.Authentication.AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is no longer valid."
                }));
        }

        // Get the scopes granted to the access token
        var scopes = User.GetScopes();

        // Build the claims based on the scopes granted
        var claims = new Dictionary<string, object>
        {
            [Claims.Subject] = user.Id.ToString()
        };

        // Profile scope claims
        if (scopes.Contains(Scopes.Profile))
        {
            claims[Claims.Name] = user.UserName ?? string.Empty;
            claims[Claims.PreferredUsername] = user.UserName ?? string.Empty;
        }

        // Email scope claims
        if (scopes.Contains(Scopes.Email))
        {
            claims[Claims.Email] = user.Email ?? string.Empty;
            claims[Claims.EmailVerified] = user.EmailConfirmed;
        }

        // Phone scope claims
        if (scopes.Contains(Scopes.Phone))
        {
            claims[Claims.PhoneNumber] = user.PhoneNumber ?? string.Empty;
            claims[Claims.PhoneNumberVerified] = user.PhoneNumberConfirmed;
        }

        // Roles scope claims (custom claim)
        if (scopes.Contains(Scopes.Roles))
        {
            var roles = await _userService.GetRoles(userId);
            claims[Claims.Role] = roles;
        }

        return Ok(claims);
    }
}
