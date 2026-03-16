using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.IdentityProvider.Services.Abstractions;

public interface IIdentityProviderManager
{
    Task<AuthApplication> GetClientAsync(string clientId, CancellationToken cancellationToken = default);

    Task<IEnumerable<AuthApplication>> GetClientsAsync(CancellationToken cancellationToken = default);

    Task<AuthApplication> CreateClientAsync(AuthApplication client, CancellationToken cancellationToken = default);

    Task<AuthApplication> UpdateClientAsync(string clientId, AuthApplication client, CancellationToken cancellationToken = default);

    Task DeleteClientAsync(string clientId, CancellationToken cancellationToken = default);

    Task<bool> IsValidRedirectUrlAsync(string clientId, string redirectUrl, CancellationToken cancellationToken = default);

    Task<int> GetIdByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
}