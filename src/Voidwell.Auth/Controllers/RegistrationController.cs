using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.Models;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Controllers;

[Route("registration")]
[SecurityHeaders]
public class RegistrationController : Controller
{
    private readonly IRegistrationService _registrationService;

    public RegistrationController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] RegistrationForm registration)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _registrationService.RegisterNewUserAsync(registration);

        return Ok();
    }
}