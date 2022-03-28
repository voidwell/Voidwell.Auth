using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Exceptions;
using Voidwell.Auth.Admin.Mappers;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.Data.Repositories;

namespace Voidwell.Auth.Admin.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repository;

        public ClientService(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<ClientApiDto> GetClientAsync(string clientId)
        {
            var id = await GetIdByClientIdAsync(clientId);
            var client = await _repository.GetClientAsync(id);
            return client.ToModel();
        }

        public async Task<PagedList<ClientApiDto>> GetClientsAsync(string search = "", int page = 1)
        {
            var clients = await _repository.GetClientsAsync(search, page);
            return new PagedList<ClientApiDto>(clients.Data.Select(c => c.ToModel()), clients.PageNumber, clients.PageSize, clients.TotalCount);
        }

        public async Task<IEnumerable<SecretApiDto>> GetClientSecretsAsync(string clientId)
        {
            var id = await GetIdByClientIdAsync(clientId);
            var secrets = await _repository.GetClientSecretsAsync(id);
            return secrets.Select(a => a.ToModel());
        }

        public async Task<ClientApiDto> CreateClientAsync(ClientApiDto client)
        {
            var existing = await _repository.GetIdFromClientId(client.ClientId);
            if (existing != null)
            {
                throw new ConflictException($"A client with id '{client.ClientId} already exists.");
            }

            var createdClient = await _repository.AddClientAsync(client.ToEntity());
            return createdClient.ToModel();
        }

        public async Task<ClientApiDto> UpdateClientAsync(string clientId, ClientApiDto client)
        {
            var id = await GetIdByClientIdAsync(clientId);
            var updatedClient = await _repository.UpdateClientAsync(id, client.ToEntity());
            return updatedClient.ToModel();
        }

        public async Task RemoveClientAsync(string clientId)
        {
            var id = await GetIdByClientIdAsync(clientId);
            await _repository.RemoveClientAsync(id);
        }

        public async Task<CreatedSecretResponse> CreateClientSecretAsync(string clientId, SecretRequest request)
        {
            var id = await GetIdByClientIdAsync(clientId);

            var secretValue = Guid.NewGuid().ToString();

            var secret = new ClientSecret
            {
                Created = DateTime.UtcNow,
                Description = request.Description,
                Expiration = request.Expiration,
                Type = "SharedSecret",
                ClientId = id,
                Value = secretValue.Sha256()
            };

            var createdSecret = await _repository.AddClientSecretAsync(id, secret);

            return new CreatedSecretResponse(secretValue, createdSecret.ToModel());
        }

        public async Task DeleteClientSecretAsync(string clientId, int secretId)
        {
            var id = await GetIdByClientIdAsync(clientId);

            var secret = await _repository.GetClientSecretAsync(id, secretId);
            if (secret == null)
            {
                return;
            }

            await _repository.DeleteClientSecretAsync(id, secretId);
        }

        private async Task<int> GetIdByClientIdAsync(string clientId)
        {
            var id = await _repository.GetIdFromClientId(clientId);
            if (id == null)
            {
                throw new NotFoundException($"Client '{clientId}' not found");
            }
            return id.Value;
        }
    }
}
