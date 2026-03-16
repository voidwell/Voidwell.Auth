using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.UserManagement.Services.Abstractions;

public interface IRoleService
{
    Task<ApplicationRole> CreateRoleAsync(string role);
    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();
    Task DeleteRoleAsync(Guid roleId);
}
