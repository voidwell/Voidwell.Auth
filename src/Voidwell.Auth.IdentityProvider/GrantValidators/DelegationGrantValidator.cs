using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Voidwell.Auth.IdentityProvider.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.IdentityProvider.GrantValidators;

internal class DelegationGrantValidator : IGrantValidator
{
    private readonly IOpenIddictTokenManager _tokenManager;
    private readonly ILogger<DelegationGrantValidator> _logger;

    public DelegationGrantValidator(
        IOpenIddictTokenManager tokenManager,
        ILogger<DelegationGrantValidator> logger)
    {
        _tokenManager = tokenManager;
        _logger = logger;
    }

    public string GrantType => "delegation";

    public async Task<GrantValidationResult> ValidateAsync(OpenIddictRequest request)
    {
        var userToken = request.Token;

        if (string.IsNullOrEmpty(userToken))
        {
            _logger.LogWarning("Null token passed to {Validator}", nameof(DelegationGrantValidator));

            return new GrantValidationResult(Errors.InvalidGrant);
        }

        var validationResult = await _tokenManager.ValidateAsync(userToken).ToListAsync();

        if (validationResult.Count != 0)
        {
            _logger.LogWarning("Delegation token validation failed: {Error}", validationResult.First().ErrorMessage);

            return new GrantValidationResult(Errors.InvalidGrant);
        }

        var payload = await _tokenManager.GetPayloadAsync(userToken);
        var claims = JsonSerializer.Deserialize<List<Claim>>(payload);

        if (claims == null || claims.Count == 0)
        {
            _logger.LogWarning("Delegation token validation failed: no claims returned");

            return new GrantValidationResult(Errors.InvalidGrant);
        }

        var subjectClaim = claims.FirstOrDefault(c => c.Type == Claims.Subject);
        if (subjectClaim == null)
        {
            var claimsList = string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}"));
            _logger.LogWarning("Delegation token validation failed: missing 'sub' claim. Available claims: {Claims}", claimsList);

            return new GrantValidationResult(Errors.InvalidGrant);
        }

        var identityProvider = claims.FirstOrDefault(c => c.Type == "idp")?.Value;

        _logger.LogInformation("Delegation GrantValidationResult complete");

        return new GrantValidationResult(subjectClaim.Value, "delegation", identityProvider: identityProvider ?? "unknown");
    }
}
