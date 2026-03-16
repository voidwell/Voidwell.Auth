using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.IdentityProvider.Services;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.Controllers;

[Route("account/login")]
public class LoginController : Controller
{
    private readonly IIdentityProviderManager _idpm;
    private readonly IAccountService _accountService;
    private readonly ICredentialSignOnService _credentialSignOnService;

    public LoginController(IIdentityProviderManager idpm, ICredentialSignOnService credentialSignOnService, IAccountService accountService)
    {
        _idpm = idpm;
        _credentialSignOnService = credentialSignOnService;
        _accountService = accountService;
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
                // Validate redirect URL if we're in an OAuth flow (has client_id)
                if (!string.IsNullOrWhiteSpace(authRequest.ClientId))
                {
                    var client = await _idpm.GetClientAsync(authRequest.ClientId);
                    if (!string.IsNullOrWhiteSpace(authRequest.ReturnUrl) && !await _idpm.IsValidRedirectUrlAsync(authRequest.ClientId, authRequest.ReturnUrl))
                    {
                        hasError = true;
                        errorMsg = string.Format("Redirect uri '{0}' is invalid for client '{1}'. Notify service administrator.", authRequest.ReturnUrl, authRequest.ClientId);
                    }
                }

                if (!hasError)
                {
                    var error = await _credentialSignOnService.AuthenticateAsync(authRequest);
                    if (error != null)
                    {
                        hasError = true;
                        errorMsg = error;
                    }
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

                return View(tryAgainView);
            }

            if (!string.IsNullOrWhiteSpace(authRequest.ReturnUrl))
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