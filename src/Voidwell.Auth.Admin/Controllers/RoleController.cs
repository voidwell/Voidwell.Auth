using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Admin.Controllers;

[Route("admin/role")]
[SecurityHeaders]
[Authorize("IsAdmin")]
public class RoleController : Controller
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRoles();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<ActionResult> AddRole([FromBody] RoleRequest roleRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var role = await _roleService.CreateRole(roleRequest.Name);
        return Created("admin/role", role);
    }

    [HttpDelete("{roleId:guid}")]
    public async Task<ActionResult> DeleteRole(Guid roleId)
    {
        await _roleService.DeleteRole(roleId);
        return NoContent();
    }
}
