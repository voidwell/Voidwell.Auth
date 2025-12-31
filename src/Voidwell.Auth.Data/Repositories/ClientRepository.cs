using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Voidwell.Auth.Data.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IdentityServerConfigurationDbContext _dbContext;

    public ClientRepository(IdentityServerConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Client> GetClientAsync(int clientId)
    {
        return _dbContext.Clients
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.ClientSecrets)
            .Include(x => x.Claims)
            .Include(x => x.IdentityProviderRestrictions)
            .Include(x => x.AllowedCorsOrigins)
            .Include(x => x.Properties)
            .Where(x => x.Id == clientId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<int?> GetIdFromClientId(string clientId)
    {
        var client = await _dbContext.Clients
            .Where(a => a.ClientId == clientId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        return client?.Id;
    }

    public async Task<PagedList<Client>> GetClientsAsync(string search = "", int page = 1, int pageSize = 10)
    {
        var queryBase = string.IsNullOrEmpty(search)
            ? _dbContext.Clients
            : _dbContext.Clients.Where(a => a.ClientId.Contains(search, StringComparison.InvariantCultureIgnoreCase) || (a .ClientName != null && a.ClientName.Contains(search, StringComparison.InvariantCultureIgnoreCase)));

        var clientsTask = queryBase
            .OrderBy(a => a.ClientId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var countTask = queryBase.CountAsync();

        await Task.WhenAll(clientsTask, countTask);

        return new PagedList<Client>(clientsTask.Result, page, pageSize, countTask.Result);
    }

    public async Task<ClientSecret> AddClientSecretAsync(int clientId, ClientSecret clientSecret)
    {
        var client = await _dbContext.Clients
            .Where(a => a.Id == clientId)
            .SingleOrDefaultAsync();

        clientSecret.Client = client;

        await _dbContext.ClientSecrets.AddAsync(clientSecret);

        await _dbContext.SaveChangesAsync();

        return clientSecret;
    }

    public async Task<IEnumerable<ClientSecret>> GetClientSecretsAsync(int clientId)
    {
        return await _dbContext.ClientSecrets
            .Where(x => x.ClientId == clientId)
            .ToListAsync();
    }

    public Task<ClientSecret> GetClientSecretAsync(int clientId, int clientSecretId)
    {
        return _dbContext.ClientSecrets
            .Include(x => x.Client)
            .Where(x => x.Id == clientSecretId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task DeleteClientSecretAsync(int clientId, int clientSecretId)
    {
        var secretToDelete = await _dbContext.ClientSecrets.Where(x => x.ClientId == clientId && x.Id == clientSecretId).SingleOrDefaultAsync();

        _dbContext.ClientSecrets.Remove(secretToDelete);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Client> AddClientAsync(Client client)
    {
        _dbContext.Clients.Add(client);

        await _dbContext.SaveChangesAsync();

        return client;
    }

    public async Task<Client> UpdateClientAsync(int clientId, Client client)
    {
        await RemoveClientRelationsAsync(clientId);

        _dbContext.Clients.Update(client);

        await _dbContext.SaveChangesAsync();

        return client;
    }

    public async Task RemoveClientAsync(int clientId)
    {
        var client = await _dbContext.Clients.Where(a => a.Id == clientId).FirstOrDefaultAsync();

        _dbContext.Clients.Remove(client);

        await _dbContext.SaveChangesAsync();
    }

    private async Task RemoveClientRelationsAsync(int clientId)
    {
        var clientClaimsTask = _dbContext.ClientClaims.Where(x => x.ClientId == clientId).ToListAsync();
        var clientScopesTask = _dbContext.ClientScopes.Where(x => x.ClientId == clientId).ToListAsync();
        var clientPropertiesTask = _dbContext.ClientProperties.Where(x => x.ClientId == clientId).ToListAsync();
        var clientGrantTypesTask = _dbContext.ClientGrantTypes.Where(x => x.ClientId == clientId).ToListAsync();
        var clientRedirectUrisTask = _dbContext.ClientRedirectUris.Where(x => x.ClientId == clientId).ToListAsync();
        var clientCorsOriginsTask = _dbContext.ClientCorsOrigins.Where(x => x.ClientId == clientId).ToListAsync();
        var clientIdPRestrictionsTask = _dbContext.ClientIdPRestrictions.Where(x => x.ClientId == clientId).ToListAsync();
        var clientPostLogoutRedirectUrisTask = _dbContext.ClientPostLogoutRedirectUris.Where(x => x.ClientId == clientId).ToListAsync();

        await Task.WhenAll(
            clientClaimsTask,
            clientScopesTask,
            clientPropertiesTask,
            clientGrantTypesTask,
            clientRedirectUrisTask,
            clientCorsOriginsTask,
            clientIdPRestrictionsTask,
            clientPostLogoutRedirectUrisTask);

        _dbContext.ClientClaims.RemoveRange(clientClaimsTask.Result);
        _dbContext.ClientScopes.RemoveRange(clientScopesTask.Result);
        _dbContext.ClientProperties.RemoveRange(clientPropertiesTask.Result);
        _dbContext.ClientGrantTypes.RemoveRange(clientGrantTypesTask.Result);
        _dbContext.ClientRedirectUris.RemoveRange(clientRedirectUrisTask.Result);
        _dbContext.ClientCorsOrigins.RemoveRange(clientCorsOriginsTask.Result);
        _dbContext.ClientIdPRestrictions.RemoveRange(clientIdPRestrictionsTask.Result);
        _dbContext.ClientPostLogoutRedirectUris.RemoveRange(clientPostLogoutRedirectUrisTask.Result);
    }
}
