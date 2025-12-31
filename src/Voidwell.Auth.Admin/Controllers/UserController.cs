using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Admin.Controllers;

[Route("admin/user")]
[SecurityHeaders]
[Authorize("IsAdmin")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<SimpleUser>>> GetAllUsers([FromQuery] int page = 1)
    {
        const int pageSize = 100;

        var users = await _userService.GetUsersAsync();
        var pageUsers = users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var pagedList = new PagedList<SimpleUser>(pageUsers, page, pageSize, users.Count());

        return Ok(pagedList);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult> GetUser(Guid userId)
    {
        var details = await _userService.GetUserDetails(userId);

        return Ok(details);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> DeleteUser(Guid userId)
    {
        await _userService.DeleteUser(userId);

        return NoContent();
    }

    [HttpPost("byemail")]
    public async Task<ActionResult> GetUserByEmail([FromBody] EmailAddressRequest emailAddress)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userService.GetUserByEmail(emailAddress.EmailAddress);

        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("{userId:guid}/name")]
    public async Task<ActionResult> GetDisplayName(Guid userId)
    {
        var displayName = await _userService.GetDisplayName(userId);

        return Ok(displayName);
    }

    [HttpPost("names")]
    public async Task<ActionResult> GetDisplayNames([FromQuery] IEnumerable<Guid> userIds)
    {
        var displayNames = await _userService.GetDisplayNames(userIds);

        return Ok(displayNames);
    }

    [HttpPut("{userId:guid}/roles")]
    public async Task<ActionResult> UpdateUserRoles(Guid userId, [FromBody] UserRolesRequest userRoles)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var roles = await _userService.UpdateRoles(userId, userRoles.Roles);
        if (roles == null)
        {
            return NotFound("User not found");
        }

        return Created($"admin/user/{userId}/roles", roles);
    }

    [HttpPost("{userId:guid}/lock")]
    public async Task<ActionResult> LockUser(Guid userId, [FromBody] UserLockRequest request)
    {
        await _userService.LockUser(userId, request?.LockLength, request?.IsPermanant);

        return NoContent();
    }

    [HttpPost("{userId:guid}/unlock")]
    public async Task<ActionResult> UnlockUser(Guid userId)
    {
        await _userService.UnlockUser(userId);

        return NoContent();
    }

    [HttpGet("{userId:guid}/roles")]
    public async Task<ActionResult> GetRolesForUser(Guid userId)
    {
        var roles = await _userService.GetRoles(userId);

        return Ok(roles);
    }
}
