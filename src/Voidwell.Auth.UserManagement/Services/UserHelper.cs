using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;
using Voidwell.Auth.UserManagement.Exceptions;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.UserManagement.Services;

public class UserHelper : IUserHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserIdFromContext()
    {
        var claimUserId = _httpContextAccessor.HttpContext.User.FindFirst(JwtClaimTypes.Subject)?.Value;

        if (!Guid.TryParse(claimUserId, out Guid userId))
        {
            throw new InvalidUserIdException();
        }

        return userId;
    }
}
