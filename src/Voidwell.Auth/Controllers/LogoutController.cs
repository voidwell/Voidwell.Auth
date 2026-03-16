using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;
using Voidwell.Auth.Models;
using Voidwell.Auth.Services.Abstractions;

namespace Voidwell.Auth.Controllers;

[Route("account/logout")]
public class LogoutController : Controller
{
    private readonly IAccountService _accountService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutController(
        IAccountService accountService,
        SignInManager<ApplicationUser> signInManager)
    {
        _accountService = accountService;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Logout() => View();

    [ActionName(nameof(Logout)), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PostLogout()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        await _signInManager.SignOutAsync();

        // Returning a SignOutResult will ask OpenIddict to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }
}