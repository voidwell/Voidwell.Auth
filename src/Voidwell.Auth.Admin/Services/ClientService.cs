using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.IdentityServer.Models;
using Voidwell.Auth.IdentityServer.Services.Abstractions;

namespace Voidwell.Auth.Admin.Services;

public class ClientService : IClientService
{
    private const int _pageSize = 50;

    private readonly IIdentityProviderManager _idpm;

    public ClientService(IIdentityProviderManager idpm)
    {
        _idpm = idpm;
    }

    public async Task<ClientApiDto> GetClientAsync(string clientId)
    {
        return await _idpm.GetClientAsync(clientId);
    }

    public async Task<PagedList<ClientApiDto>> GetClientsAsync(string search = "", int page = 1)
    {
        var results = await _idpm.GetClientsAsync();

        if (!string.IsNullOrEmpty(search))
        {
            results = results
                .Where(c => c.ClientId.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                            (c.ClientName != null && c.ClientName.Contains(search, StringComparison.InvariantCultureIgnoreCase)));
        }

        var pageList = results.Take(_pageSize).Skip(_pageSize * page);

        return new PagedList<ClientApiDto>(pageList, page, _pageSize, results.Count());
    }

    public async Task<IEnumerable<SecretApiDto>> GetClientSecretsAsync(string clientId)
    {
        return await _idpm.GetClientSecretsAsync(clientId);
    }

    public async Task<ClientApiDto> CreateClientAsync(ClientApiDto client)
    {
        return await _idpm.CreateClientAsync(client);
    }

    public async Task<ClientApiDto> UpdateClientAsync(string clientId, ClientApiDto client)
    {
        return await _idpm.UpdateClientAsync(clientId, client);
    }

    public async Task RemoveClientAsync(string clientId)
    {
        await _idpm.DeleteClientAsync(clientId);
    }

    public async Task<CreatedSecretResponse> CreateClientSecretAsync(string clientId, SecretRequest request)
    {
        var secretDto = new SecretApiDto
        {
            Description = request.Description,
            Expiration = request.Expiration,
            Type = "SharedSecret",
            Value = Guid.NewGuid().ToString(),
            Created = DateTime.UtcNow
        };

        var createdSecret = await _idpm.AddClientSecretAsync(clientId, secretDto);

        return new CreatedSecretResponse(secretDto.Value, createdSecret);
    }

    public async Task DeleteClientSecretAsync(string clientId, int secretId)
    {
        await _idpm.DeleteClientSecretAsync(clientId, secretId);
    }
}
