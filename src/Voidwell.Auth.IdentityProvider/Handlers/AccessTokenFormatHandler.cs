using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using Voidwell.Auth.Data.Entities;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace Voidwell.Auth.IdentityProvider.Handlers;

/// <summary>
/// Custom OpenIddict handler that implements per-client access token format (JWT vs reference).
/// For clients configured with AccessTokenType = "reference", this handler stores the token
/// in the database and returns a short opaque reference identifier instead of the full JWT.
/// </summary>
public class AccessTokenFormatHandler : IOpenIddictServerHandler<ProcessSignInContext>
{
    private readonly IOpenIddictApplicationStore<AuthApplication> _applicationStore;
    private readonly IOpenIddictTokenManager _tokenManager;
    private readonly ILogger<AccessTokenFormatHandler> _logger;

    public AccessTokenFormatHandler(
        IOpenIddictApplicationStore<AuthApplication> applicationStore,
        IOpenIddictTokenManager tokenManager,
        ILogger<AccessTokenFormatHandler> logger)
    {
        _applicationStore = applicationStore;
        _tokenManager = tokenManager;
        _logger = logger;
    }

    /// <summary>
    /// Gets the default descriptor for this handler.
    /// This handler runs after the default access token generation but before the response is sent.
    /// Order 500_000 ensures it runs after token generation handlers but before response handlers.
    /// </summary>
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<ProcessSignInContext>()
            .AddFilter<RequireAccessTokenGenerated>()
            .UseScopedHandler<AccessTokenFormatHandler>()
            .SetOrder(500_000)
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

            

    public async ValueTask HandleAsync(ProcessSignInContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Skip if no access token was generated
        if (string.IsNullOrEmpty(context.AccessToken))
        {
            return;
        }

        var clientId = context.Request?.ClientId;
        if (string.IsNullOrEmpty(clientId))
        {
            return;
        }

        // Look up the client's configuration
        var application = await _applicationStore.FindByClientIdAsync(clientId, context.CancellationToken);
        if (application == null)
        {
            return;
        }

        // Only convert to reference token if the client is configured for it
        if (!string.Equals(application.AccessTokenType, "reference", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _logger.LogDebug("Converting access token to reference token for client {ClientId}", clientId);

        // Generate a cryptographically secure reference identifier
        var referenceId = GenerateReferenceId();

        // Create a token entry in the database to store the actual token
        var tokenDescriptor = new OpenIddictTokenDescriptor
        {
            ApplicationId = application.Id.ToString(),
            AuthorizationId = context.AccessTokenPrincipal?.GetAuthorizationId(),
            CreationDate = DateTimeOffset.UtcNow,
            ExpirationDate = context.AccessTokenPrincipal?.GetExpirationDate(),
            Principal = context.AccessTokenPrincipal,
            ReferenceId = referenceId,
            Status = OpenIddictConstants.Statuses.Valid,
            Subject = context.AccessTokenPrincipal?.GetClaim(OpenIddictConstants.Claims.Subject),
            Type = OpenIddictConstants.TokenTypeHints.AccessToken,
            // Store the original JWT payload for introspection
            Payload = context.AccessToken
        };

        await _tokenManager.CreateAsync(tokenDescriptor, context.CancellationToken);

        // Replace the access token with the reference identifier
        // The reference ID is what gets returned to the client
        context.AccessToken = referenceId;

        _logger.LogDebug("Access token converted to reference token for client {ClientId}", clientId);
    }

    private static string GenerateReferenceId()
    {
        // Generate a 256-bit (32 byte) cryptographically secure random identifier
        // and encode it as URL-safe base64
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}

/// <summary>
/// Filter that ensures the handler only runs when an access token has been generated.
/// </summary>
public class RequireAccessTokenGenerated : IOpenIddictServerHandlerFilter<ProcessSignInContext>
{
    public ValueTask<bool> IsActiveAsync(ProcessSignInContext context)
    {
        return new ValueTask<bool>(context.GenerateAccessToken);
    }
}
