using System;

namespace Voidwell.Auth.UserManagement.Exceptions;

public class UserCreationException : Exception
{
    public UserCreationException()
    { }

    public UserCreationException(string message)
        :base(message)
    { }
}
