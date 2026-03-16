using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.IdentityProvider.Models;

namespace Voidwell.Auth.IdentityProvider.Services.Abstractions;

public interface ISecretManager
{
    Task<IEnumerable<ClientSecret>> GetClientSecretsAsync(string clientId, CancellationToken cancellationToken = default);

    Task<CreatedSecretResponse> CreateClientSecretAsync(string clientId, string description, DateTime? expiration, CancellationToken cancellationToken = default);

    Task DeleteClientSecretAsync(string clientId, int secretId, CancellationToken cancellationToken = default);
}