using System;

namespace Voidwell.Auth.UserManagement.Exceptions;

public class InvalidRoleRequestException : Exception
{
    public InvalidRoleRequestException()
    { }

    public InvalidRoleRequestException(string message)
        :base(message)
    { }
}
