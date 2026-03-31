using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Controllers;

[Route("connect/token")]
public class TokenController : ControllerBase
{
    private readonly IGrantValidatorProvider _grantValidatorProvider;

    public TokenController(IGrantValidatorProvider grantValidatorProvider)
    {
        _grantValidatorProvider = grantValidatorProvider;
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    [EnableRateLimiting("oauth")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var validator = _grantValidatorProvider.GetValidator(request.GrantType);
        if (validator == null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }

        var result = await validator.ValidateAsync(request);
        if (result.IsError)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = result.Error,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = result.ErrorDescription
                }));
        }

        return SignIn(result.Subject, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
