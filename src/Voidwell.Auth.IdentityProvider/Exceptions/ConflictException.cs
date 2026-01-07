using System;

namespace Voidwell.Auth.IdentityProvider.Exceptions;

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
