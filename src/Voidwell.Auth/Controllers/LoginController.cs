using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using Voidwell.VoidwellAuth.IdentityServer.Services;
using Voidwell.VoidwellAuth.Client.Models;

namespace Voidwell.VoidwellAuth.Client.Controllers
{
    [Route("account/login")]
    [SecurityHeaders]
    public class LoginController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly AccountService _account;
        private readonly IAuthenticationService _authService;

        public LoginController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IEventService events,
            IAuthenticationService authService)
        {
            _authService = authService;

            _interaction = interaction;
            _events = events;
            _account = new AccountService(interaction, httpContextAccessor, clientStore);
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
                // validate username/password against in-memory store
                var authResult = _authService.Authenticate(authRequest.Username, authRequest.Password);
                if (authResult != null)
                {
                    AuthenticationProperties props = null;
                    // only set explicit expiration here if persistent. 
                    // otherwise we reply upon expiration configured in cookie middleware.
                    if (AccountOptions.AllowRememberLogin && authRequest.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    };

                    // issue authentication cookie with subject ID and username
                    await _events.RaiseAsync(new UserLoginSuccessEvent(authResult.Profile.Email, authResult.UserId.ToString(), authResult.Profile.DisplayName));
                    await HttpContext.Authentication.SignInAsync(authResult.UserId.ToString(), authResult.Profile.Email, props, authResult.Claims.ToArray());

                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint or a local page
                    if (_interaction.IsValidReturnUrl(authRequest.ReturnUrl) || Url.IsLocalUrl(authRequest.ReturnUrl))
                    {
                        return Redirect(authRequest.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(authRequest.Username, "invalid credentials"));

                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await _account.BuildLoginViewModelAsync(authRequest);
            return View(vm);
        }
    }
}