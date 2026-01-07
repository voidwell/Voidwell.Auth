using System;
using System.Security.Claims;
using IdentityModel;

namespace Voidwell.Auth.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string GetSubjectId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(JwtClaimTypes.Subject) ?? throw new InvalidOperationException("subject claim is missing");
    }
    
    public static string GetDisplayName(this ClaimsPrincipal principal)
    {
        var name = principal.FindFirstValue(JwtClaimTypes.Name);
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        return string.Empty;
    }

    public static string GetUsername(this ClaimsPrincipal principal)
    {
        var email = principal.FindFirstValue(JwtClaimTypes.Email);
        if (!string.IsNullOrWhiteSpace(email))
        {
            return email;
        }

        var name = principal.GetDisplayName();
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        var subId = principal.FindFirstValue(JwtClaimTypes.Subject);
        if (!string.IsNullOrWhiteSpace(subId))
        {
            return subId;
        }
        
        return string.Empty;
    }
}
