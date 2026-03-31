using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.IdentityProvider;
using Voidwell.Auth.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Controllers;

[Route("connect/verify")]
public class VerificationController : Controller
{
    private readonly IUserService _userService;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IAntiforgery _antiforgery;

    public VerificationController(
        IUserService userService,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager,
        IAntiforgery antiforgery)
    {
        _userService = userService;
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
        _antiforgery = antiforgery;
    }

    [HttpGet, HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Verify()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // No user_code yet — show the entry form.
        if (string.IsNullOrEmpty(request.UserCode))
        {
            return View("UserCode");
        }

        // Require authentication before the user can approve device access.
        var result = await HttpContext.AuthenticateAsync();
        if (result is not { Succeeded: true })
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = $"{Request.PathBase}/connect/verify?user_code={Uri.EscapeDataString(request.UserCode)}"
            });
        }

        if (HttpContext.Request.HasFormContentType)
        {
            var confirmed = HttpContext.Request.Form["confirmed"].ToString();

            if (confirmed is "yes" or "no")
            {
                try
                {
                    await _antiforgery.ValidateRequestAsync(HttpContext);
                }
                catch (AntiforgeryValidationException)
                {
                    return BadRequest();
                }

                if (confirmed == "no")
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "The user denied the authorization request."
                        }));
                }

                // confirmed == "yes" — build identity and complete the device authorization.
                var user = await _userService.GetUser(result.Principal.GetUserId());
                if (user is null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "The user account could not be found."
                        }));
                }

                var identity = new ClaimsIdentity(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                identity.SetClaim(Claims.Subject, user.Id.ToString())
                        .SetClaim(Claims.Email, user.Email)
                        .SetClaim(Claims.EmailVerified, user.EmailConfirmed.ToString().ToLowerInvariant())
                        .SetClaim(Claims.Name, user.UserName)
                        .SetClaims(Claims.Role, [.. (await _userService.GetRoles(user.Id))]);

                identity.SetScopes(request.GetScopes());
                identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
                identity.SetDestinations(ClaimDestinations.GetDestinations);

                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
        }

        // Show the authorization confirmation page.
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId ?? string.Empty);
        var displayName = application is not null
            ? await _applicationManager.GetDisplayNameAsync(application) ?? request.ClientId
            : request.ClientId;

        return View("Verify", new VerificationViewModel
        {
            UserCode = request.UserCode,
            ApplicationName = displayName,
            Scopes = [.. request.GetScopes()]
        });
    }
}
