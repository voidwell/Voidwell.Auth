using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading.Tasks;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.Controllers;

[Route("account/login")]
public class LoginController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ICredentialSignOnService _credentialSignOnService;

    public LoginController(ICredentialSignOnService credentialSignOnService, IAccountService accountService)
    {
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
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(AuthenticationRequest authRequest)
    {
        if (ModelState.IsValid)
        {
            bool hasError = false;
            string errorMsg = null;
            try
            {
                var error = await _credentialSignOnService.AuthenticateAsync(authRequest);
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

                return View(tryAgainView);
            }

            if (!string.IsNullOrWhiteSpace(authRequest.ReturnUrl) && Url.IsLocalUrl(authRequest.ReturnUrl))
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