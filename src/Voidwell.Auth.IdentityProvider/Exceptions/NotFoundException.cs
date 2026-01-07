using System;

namespace Voidwell.Auth.IdentityProvider.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}
