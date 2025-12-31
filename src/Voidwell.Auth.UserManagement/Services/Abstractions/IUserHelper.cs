using System;

namespace Voidwell.Auth.UserManagement.Services.Abstractions;

public interface IUserHelper
{
    Guid GetUserIdFromContext();
}