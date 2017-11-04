using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;

namespace Voidwell.VoidwellAuth.Client.Controllers
{
    [Route("account/logout")]
    [SecurityHeaders]
    public class LogoutController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly AccountService _account;

        public LogoutController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IEventService events)
        {
            _interaction = interaction;
            _events = events;
            _account = new AccountService(interaction, httpContextAccessor, clientStore);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await _account.BuildLogoutViewModelAsync(logoutId);

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
            var vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);
            if (vm.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });
                try
                {
                    // hack: try/catch to handle social providers that throw
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

            // delete local authentication cookie
            await HttpContext.SignOutAsync();

            var user = HttpContext.User;
            if (user != null)
            {
                await _events.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetName()));
            }

            return View("LoggedOut", vm);
        }
    }
}