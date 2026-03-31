using System.Collections.Generic;
using System.Security.Claims;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.IdentityProvider;

public static class ClaimDestinations
{
    public static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case Claims.Name or Claims.PreferredUsername:
                if (claim.Subject.HasScope(Scopes.Profile))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Email or Claims.EmailVerified:
                if (claim.Subject.HasScope(Scopes.Email))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Role:
                if (claim.Subject.HasScope(Scopes.Roles))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }

                yield break;

            // Nonce is an anti-replay value for ID tokens — it must never appear in access tokens.
            case Claims.Nonce:
                yield return Destinations.IdentityToken;
                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
