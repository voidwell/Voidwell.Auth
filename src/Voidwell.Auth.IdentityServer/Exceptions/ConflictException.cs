using System;

namespace Voidwell.Auth.IdentityServer.Exceptions;

public class ConflictException : Exception
{
    public ConflictException()
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }
}
