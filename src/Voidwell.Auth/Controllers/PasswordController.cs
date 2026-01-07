using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.Extensions;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Controllers;

[Route("account/password")]
[SecurityHeaders]
public class PasswordController : Controller
{
    private readonly IUserService _userService;

    public PasswordController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult> PostChangePassword([FromBody] PasswordChangeRequest changeRequest)
    {
        var userId = new Guid(HttpContext.User.GetSubjectId());

        await _userService.ChangePassword(userId, changeRequest.OldPassword, changeRequest.NewPassword);

        return Ok();
    }
}
