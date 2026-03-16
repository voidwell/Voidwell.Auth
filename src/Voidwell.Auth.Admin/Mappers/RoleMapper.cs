using Voidwell.Auth.Admin.Dtos;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Admin.Mappers;

internal static class RoleMapper
{
    public static RoleDto ToDto(this ApplicationRole entity)
    {
        return new RoleDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}
