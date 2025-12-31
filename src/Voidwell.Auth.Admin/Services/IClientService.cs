using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Admin.Services;

public interface IClientService
{
    Task<ClientApiDto> GetClientAsync(string clientId);
    Task<PagedList<ClientApiDto>> GetClientsAsync(string search = "", int page = 1);
    Task<IEnumerable<SecretApiDto>> GetClientSecretsAsync(string clientId);
    Task<ClientApiDto> CreateClientAsync(ClientApiDto client);
    Task<ClientApiDto> UpdateClientAsync(string clientId, ClientApiDto client);
    Task RemoveClientAsync(string clientId);
    Task<CreatedSecretResponse> CreateClientSecretAsync(string clientId, SecretRequest request);
    Task DeleteClientSecretAsync(string clientId, int secretId);
}