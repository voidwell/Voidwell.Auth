using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Voidwell.Auth.Models;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.Controllers;

[Route("account/logout")]
[SecurityHeaders]
public class LogoutController : Controller
{
    private readonly IIdentityProviderEventService _eventService;
    private readonly IAccountService _accountService;

    public LogoutController(
        IIdentityProviderEventService eventService,
        IAccountService accountService)
    {
        _eventService = eventService;
        _accountService = accountService;
    }

    /// <summary>
    /// Show logout page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var vm = await _accountService.BuildLogoutViewModelAsync(logoutId);

        if (vm.ShowLogoutPrompt == false)
        {
            // no need to show prompt
            return await Logout(vm);
        }

        return View(vm);
    }

    /// <summary>
    /// Handle logout page postback
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutInputModel model)
    {
        var vm = await _accountService.BuildLoggedOutViewModelAsync(model.LogoutId);

        var user = HttpContext.User;
        if (user?.Identity.IsAuthenticated == true)
        {
            // delete local authentication cookie
            await HttpContext.SignOutAsync();

            // raise the logout event
            await _eventService.RaiseUserLogoutSuccessAsync(user.GetSubjectId(), user.GetUsername());
        }

        // check if we need to trigger sign-out at an upstream identity provider
        if (vm.TriggerExternalSignout)
        {
            // build a return URL so the upstream provider will redirect back
            // to us after the user has logged out. this allows us to then
            // complete our single sign-out processing.
            string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

            // hack: try/catch to handle social providers that throw
            try
            {
                await HttpContext.SignOutAsync(vm.ExternalAuthenticationScheme,
                    new AuthenticationProperties { RedirectUri = url });
            }
            catch (NotSupportedException) // this is for the external providers that don't have signout
            {
            }
            catch (InvalidOperationException) // this is for Windows/Negotiate
            {
            }
        }

        if (vm.AutomaticRedirectAfterSignOut && vm.PostLogoutRedirectUri != null)
        {
            return Redirect(vm.PostLogoutRedirectUri);
        }

        return View("LoggedOut", vm);
    }
}