using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.Auth.Stores
{
    public interface IVoidwellClientStore: IClientStore
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client> CreateClientAsync(Client client);
        Task<Client> UpdateClientAsync(string clientId, Client client);
        Task DeleteClientAsync(string clientId);
        Task<Guid> CreateSecretAsync(string clientId, string description, DateTime? expiration = null);
        Task DeleteSecretAsync(string clientId, int secretIndex);
    }
}