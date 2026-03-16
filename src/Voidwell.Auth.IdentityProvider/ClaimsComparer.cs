using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Voidwell.Auth.IdentityProvider;

internal class ClaimsComparer : EqualityComparer<Claim>
{
    public override bool Equals(Claim x, Claim y)
    {
        return string.Equals(x?.Type, y?.Type, StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(x?.Value, y?.Value, StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(x?.ValueType, y?.ValueType, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode([DisallowNull] Claim obj)
    {
        return HashCode.Combine(
            obj.Type?.ToLowerInvariant(),
            obj.Value?.ToLowerInvariant(),
            obj.ValueType?.ToLowerInvariant());
    }
}
