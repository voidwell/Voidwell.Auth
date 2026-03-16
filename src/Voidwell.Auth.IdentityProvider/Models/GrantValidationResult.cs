using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Voidwell.Auth.IdentityProvider.Models;

public class GrantValidationResult
{
    public bool IsError { get; } = true;

    public string Error { get; }

    public string ErrorDescription { get; }

    public ClaimsPrincipal Subject { get; }

    public Dictionary<string, object> TokenResponse { get; set; } = [];

    public GrantValidationResult(Dictionary<string, object> tokenResponse = null)
    {
        IsError = false;
        TokenResponse = tokenResponse;
    }

    public GrantValidationResult(ClaimsPrincipal principal, Dictionary<string, object> tokenResponse = null)
    {
        IsError = false;

        if (principal.Identities.Count() != 1)
        {
            throw new InvalidOperationException("only a single identity supported");
        }

        if (principal.FindFirst(OpenIddictConstants.Claims.Subject) == null)
        {
            throw new InvalidOperationException("sub claim is missing");
        }

        if (principal.FindFirst("idp") == null)
        {
            throw new InvalidOperationException("idp claim is missing");
        }

        if (principal.FindFirst(OpenIddictConstants.Claims.AuthenticationMethodReference) == null)
        {
            throw new InvalidOperationException("amr claim is missing");
        }

        if (principal.FindFirst(OpenIddictConstants.Claims.AuthenticationTime) == null)
        {
            throw new InvalidOperationException("auth_time claim is missing");
        }

        Subject = principal;
        TokenResponse = tokenResponse;
    }

    public GrantValidationResult(string error, string errorDescription = null, Dictionary<string, object> tokenResponse = null)
    {
        Error = error;
        ErrorDescription = errorDescription;
        TokenResponse = tokenResponse;
    }

    public GrantValidationResult(
           string subject,
           string authenticationMethod,
           IEnumerable<Claim> claims = null,
           string identityProvider = "local",
           Dictionary<string, object> tokenResponse = null)
           : this(subject, authenticationMethod, DateTime.UtcNow, claims, identityProvider, tokenResponse)
    {
    }

    public GrantValidationResult(
            string subject,
            string authenticationMethod,
            DateTime authTime,
            IEnumerable<Claim> claims = null,
            string identityProvider = "local",
            Dictionary<string, object> tokenResponse = null)
    {
        IsError = false;

        var resultClaims = new List<Claim>
            {
                new(OpenIddictConstants.Claims.Subject, subject),
                new(OpenIddictConstants.Claims.AuthenticationMethodReference, authenticationMethod),
                new("idp", identityProvider),
                new(OpenIddictConstants.Claims.AuthenticationTime, new DateTimeOffset(authTime).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

        if (claims != null && claims.Any())
        {
            resultClaims.AddRange(claims);
        }

        var id = new ClaimsIdentity(authenticationMethod);
        id.AddClaims(resultClaims.Distinct(new ClaimsComparer()));
        id.SetDestinations(ClaimDestinations.GetDestinations);

        Subject = new ClaimsPrincipal(id);
        TokenResponse = tokenResponse;
    }
}
