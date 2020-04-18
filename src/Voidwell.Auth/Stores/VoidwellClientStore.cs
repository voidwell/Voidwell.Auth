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

namespace Voidwell.Auth.Stores
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

        public async Task DeleteClientAsync(string clientId)
        {
            var existing = await _context.Clients.FindAsync(clientId);
            if (existing == null)
            {
                return;
            }

            _context.Clients.Remove(existing);
            await Context.SaveChangesAsync();
        }

        public async Task<Guid> CreateSecretAsync(string clientId, string description, DateTime? expiration = null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(a => a.ClientId == clientId);
            if (client == null)
            {
                throw new Exception();
            }

            var secretValue = Guid.NewGuid();

            var model = client.ToModel();
            model.ClientSecrets.Add(new Secret(secretValue.ToString().Sha256(), description, expiration));

            client.ClientSecrets = model.ToEntity().ClientSecrets;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return secretValue;
        }

        public async Task DeleteSecretAsync(string clientId, int secretIndex)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(a => a.ClientId == clientId);
            if (client == null)
            {
                throw new Exception();
            }

            var model = client.ToModel();
            var secret = model.ClientSecrets.ElementAt(secretIndex);
            if (secret == default(Secret))
            {
                throw new Exception();
            }

            model.ClientSecrets.Remove(secret);

            client.ClientSecrets = model.ToEntity().ClientSecrets;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
    }
}
