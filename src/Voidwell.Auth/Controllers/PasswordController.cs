using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Controllers;

[Route("account/password")]
[SecurityHeaders]
public class PasswordController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserHelper _userHelper;

    public PasswordController(IUserService userService, IUserHelper userHelper)
    {
        _userService = userService;
        _userHelper = userHelper;
    }

    [HttpPost]
    public async Task<ActionResult> PostChangePassword([FromBody] PasswordChangeRequest changeRequest)
    {
        var userId = _userHelper.GetUserIdFromContext();

        await _userService.ChangePassword(userId, changeRequest.OldPassword, changeRequest.NewPassword);

        return Ok();
    }
}
