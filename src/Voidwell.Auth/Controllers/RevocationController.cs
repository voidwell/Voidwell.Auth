using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Controllers;

[Route("connect/revocation")]
public class RevocationController : ControllerBase
{
    private readonly IOpenIddictTokenManager _tokenManager;

    public RevocationController(IOpenIddictTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    [HttpPost]
    [Produces("application/json")]
    public async Task<IActionResult> Revoke()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the token from the request
        var token = request.Token;
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new
            {
                error = Errors.InvalidRequest,
                error_description = "The token parameter is missing."
            });
        }

        // Try to find the token in the database
        var tokenEntry = await _tokenManager.FindByReferenceIdAsync(token);

        // If not found by reference ID, try to find by ID
        tokenEntry ??= await _tokenManager.FindByIdAsync(token);

        if (tokenEntry is not null)
        {
            // Revoke the token and any associated tokens (e.g., refresh tokens)
            await _tokenManager.TryRevokeAsync(tokenEntry);
        }

        // Per RFC 7009, the revocation endpoint returns 200 OK even if the token
        // doesn't exist or is already revoked to prevent token scanning attacks
        return Ok();
    }
}
