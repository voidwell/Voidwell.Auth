
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Auth.Data;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;
using Voidwell.Auth.IdentityProvider.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Voidwell.Auth.IdentityProvider.Services;

public class SecretManager : ISecretManager
{
    private readonly AuthDbContext _authDbContext;
    private readonly IIdentityProviderManager _idpm;

    public SecretManager(AuthDbContext authDbContext, IIdentityProviderManager idpm)
    {
        _authDbContext = authDbContext;
        _idpm = idpm;
    }

    public async Task<IEnumerable<ClientSecret>> GetClientSecretsAsync(string clientId, CancellationToken cancellationToken)
    {
        var id = await _idpm.GetIdByClientIdAsync(clientId, cancellationToken);

        return await _authDbContext.ClientSecrets
            .Where(a => a.ClientId == id)
            .ToListAsync(cancellationToken);
    }

    public async Task<CreatedSecretResponse> CreateClientSecretAsync(string clientId, string description, DateTime? expiration, CancellationToken cancellationToken)
    {
        var id = await _idpm.GetIdByClientIdAsync(clientId, cancellationToken);
        var secretValue = Guid.NewGuid().ToString();

        var secret = new ClientSecret
        {
            ClientId = id,
            Created = DateTime.UtcNow,
            Description = description,
            Expiration = expiration,
            Value = secretValue,
        };

        var createdSecret = await _authDbContext.ClientSecrets.AddAsync(secret, cancellationToken);
        await _authDbContext.SaveChangesAsync(cancellationToken);

        return new CreatedSecretResponse(createdSecret.Entity.Id, secretValue);
    }

    public async Task DeleteClientSecretAsync(string clientId, int secretId, CancellationToken cancellationToken)
    {
        var id = await _idpm.GetIdByClientIdAsync(clientId, cancellationToken);

        var existingSecret = await _authDbContext.ClientSecrets.FirstOrDefaultAsync(a => a.ClientId == id && a.Id == secretId, cancellationToken);
        if (existingSecret != null)
        {
            _authDbContext.ClientSecrets.Remove(existingSecret);
            await _authDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}