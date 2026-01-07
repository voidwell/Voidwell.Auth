using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;

namespace Voidwell.Auth;

public class AuthUser
{
    public string SubjectId { get; set; }

    public string Name { get; set; }

    public string IdentityProvider { get; set; }

    public ICollection<string> AuthenticationMethods { get; set; } = new HashSet<string>();

    public DateTime? AuthenticationTime { get; set; }

    public ICollection<Claim> AdditionalClaims { get; set; } = new HashSet<Claim>(new ClaimComparer());

    public AuthUser(string subjectId)
    {
        if (string.IsNullOrWhiteSpace(subjectId))
        {
            throw new ArgumentException("SubjectId is required", nameof(subjectId));
        }

        SubjectId = subjectId;
    }

    public ClaimsPrincipal CreatePrincipal()
    {
        if (string.IsNullOrWhiteSpace(SubjectId))
        {
            throw new ArgumentException("SubjectId is required", nameof(SubjectId));
        }

        List<Claim> list =
        [
            new Claim(JwtClaimTypes.Subject, SubjectId)
        ];

        if (!string.IsNullOrWhiteSpace(Name))
        {
            list.Add(new Claim(JwtClaimTypes.Name, Name));
        }

        if (!string.IsNullOrWhiteSpace(IdentityProvider))
        {
            list.Add(new Claim(JwtClaimTypes.IdentityProvider, IdentityProvider));
        }

        if (AuthenticationTime.HasValue)
        {
            list.Add(new Claim(JwtClaimTypes.AuthenticationTime, new DateTimeOffset(AuthenticationTime.Value).ToUnixTimeSeconds().ToString()));
        }

        if (AuthenticationMethods.Count != 0)
        {
            foreach (string authenticationMethod in AuthenticationMethods)
            {
                list.Add(new Claim(JwtClaimTypes.AuthenticationMethod, authenticationMethod));
            }
        }

        list.AddRange(AdditionalClaims);

        var identityClaims = list.Distinct(new ClaimComparer());

        return new ClaimsPrincipal(new ClaimsIdentity(identityClaims, "VoidwellAuth", JwtClaimTypes.Name, JwtClaimTypes.Role));
    }
}