using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.UserManagement.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<SimpleRole>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return roles.Select(a => new SimpleRole { Id = a.Id, Name = a.Name });
    }

    public async Task<SimpleRole> CreateRole(string role)
    {
        var newRole = new ApplicationRole(role);
        await _roleManager.CreateAsync(newRole);
        return new SimpleRole
        {
            Id = newRole.Id,
            Name = newRole.Name
        };
    }

    public async Task DeleteRole(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
            return;

        await _roleManager.DeleteAsync(role);
    }
}
