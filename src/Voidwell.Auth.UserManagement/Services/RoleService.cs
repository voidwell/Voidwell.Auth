using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Voidwell.Auth.UserManagement.Services.Abstractions;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.UserManagement.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.ToListAsync();
    }

    public async Task<ApplicationRole> CreateRoleAsync(string role)
    {
        var newRole = new ApplicationRole(role);

        await _roleManager.CreateAsync(newRole);

        return newRole;
    }

    public async Task DeleteRoleAsync(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return;
        }

        await _roleManager.DeleteAsync(role);
    }
}
