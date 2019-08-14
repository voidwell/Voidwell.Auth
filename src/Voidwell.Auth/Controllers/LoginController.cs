using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Voidwell.Auth;
using Voidwell.Auth.Services;

namespace Voidwell.VoidwellAuth.Client.Controllers
{
    [Route("account/login")]
    [SecurityHeaders]
    public class LoginController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAccountService _accountService;
        private readonly Auth.Services.IAuthenticationService _authenticationService;

        public LoginController(IIdentityServerInteractionService interaction, Auth.Services.IAuthenticationService authenticationService, IAccountService accountService)
        {
            _authenticationService = authenticationService;
            _interaction = interaction;
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
                try
                {
                    var error = await _authenticationService.Authenticate(authRequest);
                    if (error != null)
                    {
                        var tryAgainView = await _accountService.BuildLoginViewModelAsync(authRequest);
                        tryAgainView.Error = error;
                        return View(tryAgainView);
                    }
                }
                catch(Exception)
                {
                    var tryAgainView = await _accountService.BuildLoginViewModelAsync(authRequest);
                    return View(tryAgainView);
                }

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
}