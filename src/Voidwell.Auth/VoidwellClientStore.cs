using System;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;

namespace Voidwell.Auth
{
    public class VoidwellClientStore : ClientStore, IVoidwellClientStore
    {
        private readonly IConfigurationDbContext _context;

        public VoidwellClientStore(IConfigurationDbContext context, ILogger<VoidwellClientStore> logger):base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            var clients = await _context.Clients
                .ToListAsync();

            var models = clients.Select(a => a.ToModel()).OrderBy(a => a.ClientId);
            return models;
        }

        public async Task<Client> CreateClientAsync(Client newClient)
        {
            var existing = await FindClientByIdAsync(newClient.ClientId);
            if (existing != null)
            {
                throw new Exception();
            }

            newClient.ClientSecrets.Clear();
            var entity = newClient.ToEntity();

            _context.Clients.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToModel();
        }

        public async Task<Client> UpdateClientAsync(string clientId, Client updatedClient)
        {
            var existing = await FindClientByIdAsync(clientId);
            if (existing == null)
            {
                throw new Exception();
            }

            updatedClient.ClientSecrets = existing.ClientSecrets;
            var entity = updatedClient.ToEntity();

            _context.Clients.Update(entity);
            await _context.SaveChangesAsync();

            var savedModel = entity.ToModel();
            savedModel.ClientSecrets.Clear();

            return savedModel;
        }

        public async Task<Guid> CreateSecretAsync(string clientId, string description, DateTime? expiration = null)
        {
            var client = await FindClientByIdAsync(clientId);
            if (client == null)
            {
                throw new Exception();
            }

            if (client.ClientSecrets.Any(a => a.Description == description))
            {
                throw new Exception();
            }

            var secretValue = Guid.NewGuid();
            var secret = new Secret(secretValue.ToString(), description, expiration);

            client.ClientSecrets.Add(secret);

            var entity = client.ToEntity();
            _context.Clients.Update(entity);
            await _context.SaveChangesAsync();

            return secretValue;
        }

        public async Task DeleteSecretAsync(string clientId, string description)
        {
            var client = await FindClientByIdAsync(clientId);
            if (client == null)
            {
                throw new Exception();
            }

            var secret = client.ClientSecrets.FirstOrDefault(a => a.Description == description);
            if (secret == default(Secret))
            {
                throw new Exception();
            }

            client.ClientSecrets.Remove(secret);

            var entity = client.ToEntity();
            _context.Clients.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
