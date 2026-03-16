using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.Admin.Dtos;
using Voidwell.Auth.Admin.Mappers;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Admin.Controllers;

[Route("admin/api/user")]
[Authorize("IsAdminUser")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<UserDto>>> GetAllUsers(string search = null, int? skip = null, int? take = null)
    {
        var users = await _userService.GetUsersAsync();
        var totalCount = users.Count();

        if (!string.IsNullOrWhiteSpace(search))
        {
            users = users.Where(user =>
                (user.UserName != null && user.UserName.Contains(search, StringComparison.InvariantCultureIgnoreCase)) ||
                (user.Email != null && user.Email.Contains(search, StringComparison.InvariantCultureIgnoreCase)));
        }

        if (skip != null && take != null)
        {
            users = users.Skip(skip.Value).Take(take.Value);
        }

        var pagedList = new PagedList<UserDto>(users.Select(x => x.ToDto()), totalCount);

        return Ok(pagedList);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid userId)
    {
        var user = await _userService.GetUser(userId);
        return Ok(user.ToDto());
    }

    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> DeleteUser(Guid userId)
    {
        await _userService.DeleteUser(userId);

        return NoContent();
    }

    [HttpPost("byemail")]
    public async Task<ActionResult<UserDto>> GetUserByEmail([FromBody] EmailAddressRequest emailAddress)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userService.GetUserByEmail(emailAddress.EmailAddress);

        return user == null ? NotFound() : Ok(user.ToDto());
    }

    [HttpGet("{userId:guid}/name")]
    public async Task<ActionResult<DisplayName>> GetDisplayName(Guid userId)
    {
        var displayName = await _userService.GetDisplayName(userId);

        return Ok(displayName);
    }

    [HttpPost("names")]
    public async Task<ActionResult<IEnumerable<DisplayName>>> GetDisplayNames(IEnumerable<Guid> userIds)
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
    public async Task<ActionResult<IEnumerable<string>>> GetRolesForUser(Guid userId)
    {
        var roles = await _userService.GetRoles(userId);

        return Ok(roles);
    }
}
