using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.IdentityServer.Services.Abstractions;

public interface IIdentityProviderManager
{
    Task<ClientApiDto> GetClientAsync(string clientId);

    Task<IEnumerable<ClientApiDto>> GetClientsAsync();

    Task<IEnumerable<SecretApiDto>> GetClientSecretsAsync(string clientId);

    Task<ClientApiDto> CreateClientAsync(ClientApiDto client);

    Task<ClientApiDto> UpdateClientAsync(string clientId, ClientApiDto client);

    Task DeleteClientAsync(string clientId);

    Task<SecretApiDto> AddClientSecretAsync(string clientId, SecretApiDto request);

    Task DeleteClientSecretAsync(string clientId, int secretId);

    Task<ApiResourceApiDto> GetApiResourceAsync(string name);

    Task<IEnumerable<ApiResourceApiDto>> GetApiResourcesAsync();

    Task<IEnumerable<ApiResourceApiDto>> GetEnabledApiResourcesByScopeAsync(IEnumerable<string> scopeNames);

    Task<ApiResourceApiDto> CreateApiResourceAsync(ApiResourceApiDto apiResource);

    Task<ApiResourceApiDto> UpdateApiResourceAsync(string name, ApiResourceApiDto apiResource);

    Task DeleteApiResourceAsync(string name);
    
    Task<IEnumerable<SecretApiDto>> GetApiResourceSecretsAsync(string name);

    Task<SecretApiDto> AddApiResourceSecretAsync(string name, SecretApiDto request);

    Task DeleteApiResourceSecretAsync(string name, int secretId);

    Task<IEnumerable<IdentityResourceDto>> GetEnabledIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames);
}