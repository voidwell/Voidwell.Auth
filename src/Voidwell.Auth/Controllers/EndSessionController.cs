using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.Models;
using Voidwell.Auth.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Controllers;

[Route("connect/endsession")]
public class EndSessionController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAntiforgery _antiforgery;
    private readonly ILogoutNotificationService _logoutNotificationService;

    public EndSessionController(
        SignInManager<ApplicationUser> signInManager,
        IAntiforgery antiforgery,
        ILogoutNotificationService logoutNotificationService)
    {
        _signInManager = signInManager;
        _antiforgery = antiforgery;
        _logoutNotificationService = logoutNotificationService;
    }

    [HttpGet, HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> EndSession()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // If the user is not authenticated there is nothing to confirm — sign out silently.
        if (User?.Identity?.IsAuthenticated != true)
        {
            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = request.PostLogoutRedirectUri
                });
        }

        // If an id_token_hint was provided, verify it belongs to the currently authenticated user.
        if (!string.IsNullOrEmpty(request.IdTokenHint) && !IdTokenHintMatchesCurrentUser(request.IdTokenHint))
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The id_token_hint does not match the current user's session."
                }));
        }

        // If the user submitted the confirmation form, validate CSRF then sign out.
        if (HttpContext.Request.HasFormContentType &&
            HttpContext.Request.Form["confirmed"] == "true")
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return BadRequest();
            }

            var subjectId = User.GetSubjectId();

            // Collect front-channel URIs before signing out (while authorizations are still valid).
            var frontChannelUris = await _logoutNotificationService.GetFrontChannelLogoutUrisAsync(subjectId);

            await _signInManager.SignOutAsync();

            // Send back-channel logout notifications fire-and-forget style (errors are logged internally).
            await _logoutNotificationService.SendBackChannelLogoutNotificationsAsync(subjectId);

            if (frontChannelUris.Count > 0)
            {
                return View("FrontChannelLogout", new FrontChannelLogoutViewModel
                {
                    FrontChannelLogoutUris = frontChannelUris,
                    PostLogoutRedirectUri = request.PostLogoutRedirectUri
                });
            }

            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = request.PostLogoutRedirectUri
                });
        }

        // Show the logout confirmation page.
        return View("EndSession");
    }

    private bool IdTokenHintMatchesCurrentUser(string idTokenHint)
    {
        try
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(idTokenHint);
            return string.Equals(token.Subject, User.GetSubjectId(), StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }
}
