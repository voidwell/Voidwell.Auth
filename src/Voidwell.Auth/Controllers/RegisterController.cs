using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.Auth.Services;
using Voidwell.Auth.Models;
using Microsoft.AspNetCore.Http;

namespace Voidwell.VoidwellAuth.Client.Controllers
{
    [Route("account/register")]
    public class RegisterController : Controller
    {
        private readonly IRegistrationService _registrationService;

        public RegisterController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody]RegistrationForm registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _registrationService.RegisterNewUserAsync(registration);

            return Created("/register", user);
        }
    }
}
