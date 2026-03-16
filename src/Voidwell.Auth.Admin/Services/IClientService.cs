using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Dtos;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.IdentityProvider.Models;

namespace Voidwell.Auth.Admin.Services;

public interface IClientService
{
    Task<AuthApplicationDto> GetClientAsync(string clientId);

    Task<PagedList<AuthApplicationDto>> GetClientsAsync(string search = null, int? skip = null, int? take = null);

    Task<IEnumerable<ClientSecretDto>> GetClientSecretsAsync(string clientId);

    Task<AuthApplicationDto> CreateClientAsync(AuthApplicationDto client);

    Task<AuthApplicationDto> UpdateClientAsync(string clientId, AuthApplicationDto client);

    Task RemoveClientAsync(string clientId);

    Task<CreatedSecretResponse> CreateClientSecretAsync(string clientId, SecretRequest request);

    Task DeleteClientSecretAsync(string clientId, int secretId);
}