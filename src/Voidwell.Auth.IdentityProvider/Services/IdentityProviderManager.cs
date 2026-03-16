using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.IdentityProvider.Exceptions;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider.Services;

public class IdentityProviderManager : IIdentityProviderManager
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictApplicationStore<AuthApplication> _authStore;

    public IdentityProviderManager(IOpenIddictApplicationManager applicationManager, IOpenIddictApplicationStore<AuthApplication> authStore)
    {
        _applicationManager = applicationManager;
        _authStore = authStore;
    }

    public async Task<AuthApplication> GetClientAsync(string clientId, CancellationToken cancellationToken)
    {
        return await _authStore.FindByClientIdAsync(clientId, cancellationToken);
    }

    public async Task<IEnumerable<AuthApplication>> GetClientsAsync(CancellationToken cancellationToken)
    {
        return await _authStore.ListAsync(null, null, cancellationToken).ToListAsync(cancellationToken);
    }

    public async Task<AuthApplication> CreateClientAsync(AuthApplication client, CancellationToken cancellationToken)
    {
        var existing = await _authStore.FindByClientIdAsync(client.ClientId, cancellationToken);
        if (existing != null)
        {
            throw new ConflictException($"A client with id '{client.ClientId}' already exists.");
        }

        await _authStore.CreateAsync(client, cancellationToken);

        return await _authStore.FindByClientIdAsync(client.ClientId, cancellationToken);
    }

    public async Task<AuthApplication> UpdateClientAsync(string clientId, AuthApplication client, CancellationToken cancellationToken)
    {
        var existingClient = await _authStore.FindByClientIdAsync(clientId, cancellationToken)
            ?? throw new NotFoundException($"Client '{clientId}' not found");

        // Update properties on the existing tracked entity
        existingClient.ClientId = client.ClientId;
        existingClient.DisplayName = client.DisplayName;
        existingClient.ClientType = client.ClientType;
        existingClient.Description = client.Description;
        existingClient.ClientUri = client.ClientUri;
        existingClient.ClientLogoUri = client.ClientLogoUri;
        existingClient.ConsentType = client.ConsentType;
        existingClient.Enabled = client.Enabled;
        existingClient.AllowRememberConsent = client.AllowRememberConsent;
        existingClient.AllowOfflineAccess = client.AllowOfflineAccess;
        existingClient.AccessTokenType = client.AccessTokenType;
        existingClient.EnableLocalLogin = client.EnableLocalLogin;
        existingClient.AllowedCorsOrigins = client.AllowedCorsOrigins;
        existingClient.RedirectUris = client.RedirectUris;
        existingClient.PostLogoutRedirectUris = client.PostLogoutRedirectUris;
        existingClient.Settings = JsonPatch(existingClient.Settings, client.Settings);
        existingClient.Permissions = JsonPatch(existingClient.Permissions, client.Permissions);
        existingClient.Properties = JsonPatch(existingClient.Properties, client.Properties);

        await _authStore.UpdateAsync(existingClient, cancellationToken);

        return await _authStore.FindByClientIdAsync(existingClient.ClientId, cancellationToken);
    }

    public async Task DeleteClientAsync(string clientId, CancellationToken cancellationToken)
    {
        var client = await _authStore.FindByClientIdAsync(clientId, cancellationToken);

        await _authStore.DeleteAsync(client, cancellationToken);
    }

    public async Task<bool> IsValidRedirectUrlAsync(string clientId, string redirectUrl, CancellationToken cancellationToken)
    {
        return await _applicationManager.ValidateRedirectUriAsync(clientId, redirectUrl, cancellationToken);
    }

    public async Task<int> GetIdByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var client = await _authStore.FindByClientIdAsync(clientId, cancellationToken)
            ?? throw new NotFoundException($"Client '{clientId}' not found");

        return client.Id;
    }

    private static string JsonPatch(string targetJson, string patchJson)
    {
        // If patch is null/empty, return target as-is
        if (string.IsNullOrEmpty(patchJson))
        {
            return targetJson;
        }

        // If target is null/empty, return patch as the new value
        if (string.IsNullOrEmpty(targetJson))
        {
            return patchJson;
        }

        static JsonNode jsonPatch(JsonNode target, JsonNode patch)
        {
            if (patch is JsonObject patchObj && target is JsonObject targetObj)
            {
                foreach (var kvp in patchObj)
                {
                    var key = kvp.Key;
                    var patchValue = kvp.Value;

                    if (targetObj.TryGetPropertyValue(key, out var targetValue)
                        && targetValue is JsonObject targetChild
                        && patchValue is JsonObject patchChild)
                    {
                        // Recursive merge
                        jsonPatch(targetChild, patchChild);
                    }
                    else
                    {
                        // Add or overwrite
                        targetObj[key] = patchValue.DeepClone();
                    }
                }

                return targetObj;
            }

            // If patch isn't an object, replace target entirely
            return patch.DeepClone();
        }

        var target = JsonNode.Parse(targetJson);
        var patch = JsonNode.Parse(patchJson);
        return jsonPatch(target, patch).ToJsonString();
    }
}