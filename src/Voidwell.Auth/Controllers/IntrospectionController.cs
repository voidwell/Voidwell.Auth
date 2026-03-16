using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Controllers;

[Route("connect/introspect")]
public class IntrospectionController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictTokenManager _tokenManager;

    public IntrospectionController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictTokenManager tokenManager)
    {
        _applicationManager = applicationManager;
        _tokenManager = tokenManager;
    }

    [HttpPost]
    [Produces("application/json")]
    public async Task<IActionResult> Introspect()
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

        // Try to find the token in the database by reference ID
        var tokenEntry = await _tokenManager.FindByReferenceIdAsync(token);

        // If not found by reference ID, try to find by ID
        tokenEntry ??= await _tokenManager.FindByIdAsync(token);

        if (tokenEntry is null)
        {
            // Token not found - return inactive
            return Ok(new Dictionary<string, object>
            {
                [Claims.Active] = false
            });
        }

        // Check if the token is still valid
        var status = await _tokenManager.GetStatusAsync(tokenEntry);
        if (status != Statuses.Valid)
        {
            // Token is not valid (revoked, expired, etc.)
            return Ok(new Dictionary<string, object>
            {
                [Claims.Active] = false
            });
        }

        // Get token details
        var subject = await _tokenManager.GetSubjectAsync(tokenEntry);
        var type = await _tokenManager.GetTypeAsync(tokenEntry);
        var expirationDate = await _tokenManager.GetExpirationDateAsync(tokenEntry);
        var applicationId = await _tokenManager.GetApplicationIdAsync(tokenEntry);

        // Build the introspection response
        var response = new Dictionary<string, object>
        {
            [Claims.Active] = true,
            [Claims.Subject] = subject,
            [Claims.TokenType] = type
        };

        if (expirationDate.HasValue)
        {
            response[Claims.ExpiresAt] = expirationDate.Value.ToUnixTimeSeconds();
        }

        if (!string.IsNullOrEmpty(applicationId))
        {
            var application = await _applicationManager.FindByIdAsync(applicationId);
            if (application is not null)
            {
                response[Claims.ClientId] = await _applicationManager.GetClientIdAsync(application);
            }
        }

        return Ok(response);
    }
}
