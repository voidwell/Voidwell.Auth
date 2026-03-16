using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Voidwell.Auth.IdentityProvider.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.IdentityProvider.GrantValidators;

internal class ClientCredentialsGrantValidator : IGrantValidator
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public ClientCredentialsGrantValidator(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager)
    {
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
    }

    public string GrantType => GrantTypes.ClientCredentials;

    public async Task<GrantValidationResult> ValidateAsync(OpenIddictRequest request)
    {
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId);
        if (application == null)
        {
            return new GrantValidationResult(Errors.InvalidClient);
        }

        var clientId = await _applicationManager.GetClientIdAsync(application);
        var displayName = await _applicationManager.GetDisplayNameAsync(application);

        var scopes = request.GetScopes();
        var resources = await _scopeManager.ListResourcesAsync(scopes).ToListAsync();

        var claims = new List<Claim>
        {
            new(Claims.Subject, clientId),
            new(Claims.Name, displayName ?? clientId)
        };

        foreach (var scope in scopes)
        {
            claims.Add(new Claim(Claims.Private.Scope, scope));
        }

        foreach (var resource in resources)
        {
            claims.Add(new Claim(Claims.Private.Audience, resource));
        }

        return new GrantValidationResult(clientId, GrantTypes.ClientCredentials, claims);
    }
}
