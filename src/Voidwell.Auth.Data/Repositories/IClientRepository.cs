using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Voidwell.Auth.Data.Repositories
{
    public interface IClientRepository
    {
        Task<Client> GetClientAsync(int clientId);
        Task<int?> GetIdFromClientId(string clientId);
        Task<PagedList<Client>> GetClientsAsync(string search = "", int page = 1, int pageSize = 10);
        Task<ClientSecret> AddClientSecretAsync(int clientId, ClientSecret clientSecret);
        Task<IEnumerable<ClientSecret>> GetClientSecretsAsync(int clientId);
        Task<ClientSecret> GetClientSecretAsync(int clientId, int clientSecretId);
        Task DeleteClientSecretAsync(int clientId, int clientSecretId);
        Task<Client> AddClientAsync(Client client);
        Task<Client> UpdateClientAsync(int clientId, Client client);
        Task RemoveClientAsync(int clientId);
    }
}