using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Voidwell.Auth.Services.Abstractions;

namespace Voidwell.Auth.Controllers
{
    [Route("consent")]
    [SecurityHeaders]
    public class ConsentController : Controller
    {
        private readonly IConsentHandler _consentService;

        public ConsentController(IConsentHandler consentService)
        {
            _consentService = consentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await _consentService.BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await _consentService.ProcessConsent(model);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return View("Index", result.ViewModel);
            }

            return View("Error");
        }
    }
}
