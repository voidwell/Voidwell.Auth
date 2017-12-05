using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Microsoft.AspNetCore.Authentication;

namespace Voidwell.VoidwellAuth.Client.Controllers
{
    [Route("account/login")]
    [SecurityHeaders]
    public class LoginController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly AccountService _account;
        private readonly Auth.Services.IAuthenticationService _authenticationService;

        public LoginController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IEventService events,
            Auth.Services.IAuthenticationService authenticationService,
            IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationService = authenticationService;

            _interaction = interaction;
            _events = events;
            _account = new AccountService(interaction, httpContextAccessor, clientStore, authenticationSchemeProvider);
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = await _account.BuildLoginViewModelAsync(returnUrl);

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
                    await _authenticationService.Authenticate(authRequest);
                }
                catch(Exception)
                {
                    var tryAgainView = await _account.BuildLoginViewModelAsync(authRequest);
                    return View(tryAgainView);
                }

                if (_interaction.IsValidReturnUrl(authRequest.ReturnUrl) || Url.IsLocalUrl(authRequest.ReturnUrl))
                {
                    return Redirect(authRequest.ReturnUrl);
                }

                return Redirect("~/");
            }

            // something went wrong, show form with error
            var vm = await _account.BuildLoginViewModelAsync(authRequest);
            return View(vm);
        }
    }
}