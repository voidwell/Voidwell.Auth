using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Dtos;
using Voidwell.Auth.Admin.Mappers;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.IdentityProvider.Models;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.Admin.Services;

public class ClientService : IClientService
{
    private readonly IIdentityProviderManager _idpm;
    private readonly ISecretManager _secretManager;

    public ClientService(IIdentityProviderManager idpm, ISecretManager secretManager)
    {
        _idpm = idpm;
        _secretManager = secretManager;
    }

    public async Task<AuthApplicationDto> GetClientAsync(string clientId)
    {
        var client = await _idpm.GetClientAsync(clientId);
        return client.ToDto();
    }

    public async Task<PagedList<AuthApplicationDto>> GetClientsAsync(string search = null, int? skip = null, int? take = null)
    {
        var results = await _idpm.GetClientsAsync();
        var totalCount = results.Count();

        if (!string.IsNullOrWhiteSpace(search))
        {
            results = results
                .Where(c => c.ClientId.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                            (c.DisplayName != null && c.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)));
        }

        if (skip != null && take != null)
        {
            results = results.Skip(skip.Value).Take(take.Value);
        }

        var dtoResults = results.Select(x => x.ToDto());
        return new PagedList<AuthApplicationDto>(dtoResults, totalCount);
    }

    public async Task<IEnumerable<ClientSecretDto>> GetClientSecretsAsync(string clientId)
    {
        var secrets = await _secretManager.GetClientSecretsAsync(clientId);
        return secrets.Select(x => x.ToDto());
    }

    public async Task<AuthApplicationDto> CreateClientAsync(AuthApplicationDto clientDto)
    {
        var createdClient = await _idpm.CreateClientAsync(clientDto.ToEntity());
        return createdClient.ToDto();
    }

    public async Task<AuthApplicationDto> UpdateClientAsync(string clientId, AuthApplicationDto clientDto)
    {
        var updatedClient = await _idpm.UpdateClientAsync(clientId, clientDto.ToEntity());
        return updatedClient.ToDto();
    }

    public async Task RemoveClientAsync(string clientId)
    {
        await _idpm.DeleteClientAsync(clientId);
    }

    public async Task<CreatedSecretResponse> CreateClientSecretAsync(string clientId, SecretRequest request)
    {
        return await _secretManager.CreateClientSecretAsync(clientId, request.Description, request.Expiration);
    }

    public async Task DeleteClientSecretAsync(string clientId, int secretId)
    {
        await _secretManager.DeleteClientSecretAsync(clientId, secretId);
    }
}
