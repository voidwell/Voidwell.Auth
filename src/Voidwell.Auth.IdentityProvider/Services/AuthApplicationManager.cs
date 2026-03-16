using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider.Services;

public class AuthApplicationManager : OpenIddictApplicationManager<AuthApplication>
{
    private readonly ISecretManager _secretManager;

    public AuthApplicationManager(
        IOpenIddictApplicationCache<AuthApplication> cache,
        ILogger<OpenIddictApplicationManager<AuthApplication>> logger,
        IOptionsMonitor<OpenIddictCoreOptions> options,
        IOpenIddictApplicationStore<AuthApplication> store,
        ISecretManager secretManager)
        : base(cache, logger, options, store)
    {
        _secretManager = secretManager;
    }

    public override async ValueTask CreateAsync(AuthApplication application, string secret, CancellationToken cancellationToken = default)
    {
        await base.CreateAsync(application, secret, cancellationToken);

        if (!string.IsNullOrWhiteSpace(secret))
        {
            await _secretManager.CreateClientSecretAsync(application.ClientId, "Initial secret", null, cancellationToken);
        }
    }

    protected override async ValueTask<string> ObfuscateClientSecretAsync(string secret, CancellationToken cancellationToken = default)
    {
        return secret;
    }

    public override async ValueTask<bool> ValidateClientSecretAsync(AuthApplication application, string secret, CancellationToken cancellationToken = default)
    {
        var clientSecrets = await _secretManager.GetClientSecretsAsync(application.ClientId, cancellationToken);
        var validSecrets = clientSecrets.Where(a => a.Expiration == null || a.Expiration > DateTime.UtcNow).ToList();

        if (validSecrets.Count == 0)
        {
            Logger.LogInformation("No secrets found for client id '{ClientId'}", application.ClientId);
            return false;
        }

        foreach (var validSecret in validSecrets)
        {
            if (await ValidateClientSecretAsync(secret, validSecret.Value, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    protected override async ValueTask<bool> ValidateClientSecretAsync(string secret, string comparand, CancellationToken cancellationToken = default)
    {
        return string.Equals(secret, comparand);
    }
}
