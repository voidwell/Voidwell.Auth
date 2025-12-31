using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.UserManagement.Services.Abstractions;

public interface IRoleService
{
    Task<SimpleRole> CreateRole(string role);
    Task<IEnumerable<SimpleRole>> GetAllRoles();
    Task DeleteRole(Guid roleId);
}
