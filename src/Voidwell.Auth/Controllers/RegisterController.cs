using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.VoidwellAuth.Client.Models;
using Voidwell.VoidwellAuth.IdentityServer.Services;

namespace Voidwell.VoidwellAuth.Client.Controllers
{
    [Route("account/register")]
    public class RegisterController : Controller
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegistrationForm registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUser(registration.Username, registration.Email, registration.Password);

            return Created("/register", user);
        }
    }
}
