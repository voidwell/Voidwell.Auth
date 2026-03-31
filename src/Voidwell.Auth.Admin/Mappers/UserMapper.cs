using System;
using Voidwell.Auth.Admin.Dtos;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Admin.Mappers;

public static class UserMapper
{
    public static UserDto ToDto<T>(this T entity) where T : ApplicationUser
    {
        return new UserDto
        {
            Id = entity.Id,
            UserName = entity.UserName,
            Email = entity.Email,
            EmailConfirmed = entity.EmailConfirmed,
            PhoneNumber = entity.PhoneNumber,
            PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
            TimeZone = entity.TimeZone,
            CreatedDate = entity.CreatedDate,
            LastUpdatedDate = entity.LastUpdatedDate,
            LastLoginDate = entity.LastLoginDate,
            Birthdate = entity.Birthdate,
            PasswordSetDate = entity.PasswordSetDate,
            LockoutEndDate = entity.LockoutEnd,
            LockoutEnabled = entity.LockoutEnd.HasValue && entity.LockoutEnd > DateTimeOffset.UtcNow,
            Roles = entity is ApplicationUserWithRoles userWithRoles ? userWithRoles.Roles : []
        };
    }
}
