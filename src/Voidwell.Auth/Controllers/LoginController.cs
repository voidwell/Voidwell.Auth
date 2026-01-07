using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.IdentityServer.Services.Abstractions;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.Controllers;

[Route("account/login")]
[SecurityHeaders]
public class LoginController : Controller
{
    private readonly IIdentityProviderInteractionService _interaction;
    private readonly IAccountService _accountService;
    private readonly ICredentialSignOnService _credentialSignOnService;
    private readonly IIdentityProviderEventService _eventService;

    public LoginController(IIdentityProviderInteractionService interaction, ICredentialSignOnService credentialSignOnService,
        IAccountService accountService, IIdentityProviderEventService eventService)
    {
        _credentialSignOnService = credentialSignOnService;
        _interaction = interaction;
        _accountService = accountService;
        _eventService = eventService;
    }

    /// <summary>
    /// Show login page
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var vm = await _accountService.BuildLoginViewModelAsync(returnUrl);

        return View(vm);
    }

    /// <summary>
    /// Handle postback from username/password login
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AuthenticationRequest authRequest)
    {
        if (ModelState.IsValid)
        {
            bool hasError = false;
            string errorMsg = null;
            try
            {
                var error = await _credentialSignOnService.Authenticate(authRequest);
                if (error != null)
                {
                    hasError = true;
                    errorMsg = error;
                }
            }
            catch(Exception)
            {
                hasError = true;
                errorMsg = "An unexpected error occurred. Please try again.";
            }

            if (hasError)
            {
                var tryAgainView = await _accountService.BuildLoginViewModelAsync(authRequest);
                tryAgainView.Error = errorMsg;

                // raise the login error event
                await _eventService.RaiseUserLoginFailureAsync(authRequest.Username, errorMsg, authRequest.ClientId);

                return View(tryAgainView);
            }

            var user = HttpContext.User;

            // raise the login event
            await _eventService.RaiseUserLoginSuccessAsync(authRequest.Username, user.GetSubjectId(), user.GetUsername(), authRequest.ClientId);

            if (_interaction.IsValidReturnUrl(authRequest.ReturnUrl) || Url.IsLocalUrl(authRequest.ReturnUrl))
            {
                return Redirect(authRequest.ReturnUrl);
            }

            return Redirect("~/");
        }

        // something went wrong, show form with error
        var vm = await _accountService.BuildLoginViewModelAsync(authRequest);
        return View(vm);
    }
}