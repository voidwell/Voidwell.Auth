using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Voidwell.Auth.Data.Repositories;
using Voidwell.Auth.IdentityServer.EntityMappings;
using Voidwell.Auth.IdentityServer.Exceptions;
using Voidwell.Auth.IdentityServer.Models;
using Voidwell.Auth.IdentityServer.Services.Abstractions;

namespace Voidwell.Auth.IdentityServer.Services;

public class IdentityProviderManager : IIdentityProviderManager
{
    private readonly IClientRepository _clientRepository;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IIdentityResourceRepository _identityResourceRepository;

    public IdentityProviderManager(IClientRepository clientRepository, IApiResourceRepository apiResourceRepository, IIdentityResourceRepository identityResourceRepository)
    {
        _clientRepository = clientRepository;
        _apiResourceRepository = apiResourceRepository;
        _identityResourceRepository = identityResourceRepository;
    }

    public async Task<ClientApiDto> GetClientAsync(string clientId)
    {
        var id = await GetIdByClientIdAsync(clientId);
        var client = await _clientRepository.GetClientAsync(id);
        return client.ToModel();
    }

    public async Task<IEnumerable<ClientApiDto>> GetClientsAsync()
    {
        var clients = await _clientRepository.GetClientsAsync();
        return clients.Select(c => c.ToModel());
    }

    public async Task<ClientApiDto> CreateClientAsync(ClientApiDto client)
    {
        var existing = await _clientRepository.GetIdFromClientId(client.ClientId);
        if (existing != null)
        {
            throw new ConflictException($"A client with id '{client.ClientId} already exists.");
        }

        var createdClient = await _clientRepository.AddClientAsync(client.ToEntity());
        return createdClient.ToModel();
    }

    public async Task<ClientApiDto> UpdateClientAsync(string clientId, ClientApiDto client)
    {
        var id = await GetIdByClientIdAsync(clientId);
        var updatedClient = await _clientRepository.UpdateClientAsync(id, client.ToEntity());
        return updatedClient.ToModel();
    }

    public async Task DeleteClientAsync(string clientId)
    {
        var id = await GetIdByClientIdAsync(clientId);
        await _clientRepository.RemoveClientAsync(id);
    }

    public async Task<IEnumerable<SecretApiDto>> GetClientSecretsAsync(string clientId)
    {
        var id = await GetIdByClientIdAsync(clientId);
        var secrets = await _clientRepository.GetClientSecretsAsync(id);
        return secrets.Select(s => s.ToModel());
    }

    public async Task<SecretApiDto> AddClientSecretAsync(string clientId, SecretApiDto request)
    {
        var id = await GetIdByClientIdAsync(clientId);
        
        var secret = request.ToClientEntity(id);
        secret.Value = secret.Value.Sha256();

        var createdSecret = await _clientRepository.AddClientSecretAsync(id, secret);
        return createdSecret.ToModel();
    }

    public async Task DeleteClientSecretAsync(string clientId, int secretId)
    {
        var id = await GetIdByClientIdAsync(clientId);
        await _clientRepository.DeleteClientSecretAsync(id, secretId);
    }

    public async Task<ApiResourceApiDto> GetApiResourceAsync(string name)
    {
        var id = await GetIdByApiResourceNameAsync(name);
        var apiResource = await _apiResourceRepository.GetApiResourceAsync(id);
        return apiResource.ToModel();
    }

    public async Task<IEnumerable<ApiResourceApiDto>> GetApiResourcesAsync()
    {
        var apiResources = await _apiResourceRepository.GetApiResourcesAsync();
        return apiResources.Select(a => a.ToModel());
    }

    public async Task<IEnumerable<ApiResourceApiDto>> GetEnabledApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
    {
        var apiResources = await _apiResourceRepository.GetApiResourcesAsync();
        apiResources = apiResources.Where(r => r.Enabled && r.Scopes.Any(s => scopeNames.Contains(s.Name)));
        return apiResources.Select(a => a.ToModel());
    }

    public async Task<ApiResourceApiDto> CreateApiResourceAsync(ApiResourceApiDto apiResource)
    {
        var existing = await _apiResourceRepository.GetIdFromApiResourceName(apiResource.Name);
        if (existing != null)
        {
            throw new ConflictException($"A apiResource with name '{apiResource.Name} already exists.");
        }

        var createdResource = await _apiResourceRepository.AddApiResourceAsync(apiResource.ToEntity());
        return createdResource.ToModel();
    }

    public async Task<ApiResourceApiDto> UpdateApiResourceAsync(string name, ApiResourceApiDto apiResource)
    {
        var id = await GetIdByApiResourceNameAsync(name);

        var scopeNames = apiResource.Scopes.Select(x => x.Name).ToArray();
        var scopeConflicts = await _apiResourceRepository.GetApiResourceScopeConflictsAsync(id, scopeNames);
        if (scopeConflicts.Any())
        {
            throw new ConflictException($"A scope with this name already exists: '{string.Join(", ", scopeConflicts)}'");
        }

        var updatedApiResource = await _apiResourceRepository.UpdateApiResourceAsync(id, apiResource.ToEntity());
        return updatedApiResource.ToModel();
    }

    public async Task DeleteApiResourceAsync(string name)
    {
        var id = await GetIdByApiResourceNameAsync(name);
        await _apiResourceRepository.RemoveApiResourceAsync(id);
    }

    public async Task<IEnumerable<SecretApiDto>> GetApiResourceSecretsAsync(string name)
    {
        var id = await GetIdByApiResourceNameAsync(name);
        var secrets = await _apiResourceRepository.GetApiResourceSecretsAsync(id);
        return secrets.Select(s => s.ToModel());
    }

    public async Task<SecretApiDto> AddApiResourceSecretAsync(string name, SecretApiDto request)
    {
        var id = await GetIdByApiResourceNameAsync(name);
        
        var secret = request.ToApiEntity(id);
        secret.Value = secret.Value.Sha256();

        var createdSecret = await _apiResourceRepository.AddApiResourceSecretAsync(id, secret);
        return createdSecret.ToModel();
    }

    public async Task DeleteApiResourceSecretAsync(string name, int secretId)
    {
        var id = await GetIdByApiResourceNameAsync(name);
        await _apiResourceRepository.DeleteApiResourceSecretAsync(id, secretId);
    }

    public async Task<IEnumerable<IdentityResourceDto>> GetEnabledIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
    {
        var identityResources = await _identityResourceRepository.GetIdentityResourcesAsync();
        identityResources = identityResources.Where(r => r.Enabled && scopeNames.Contains(r.Name));
        return identityResources.Select(a => a.ToModel());
    }

    private async Task<int> GetIdByClientIdAsync(string clientId)
    {
        return await _clientRepository.GetIdFromClientId(clientId)
            ?? throw new NotFoundException($"Client '{clientId}' not found");
    }

    private async Task<int> GetIdByApiResourceNameAsync(string name)
    {
        return await _apiResourceRepository.GetIdFromApiResourceName(name)
            ?? throw new NotFoundException($"ApiResource '{name}' not found");
    }
}