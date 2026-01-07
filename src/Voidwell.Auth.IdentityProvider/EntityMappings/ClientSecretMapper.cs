using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.IdentityProvider.Models;

namespace Voidwell.Auth.IdentityProvider.EntityMappings;

internal static class ClientSecretMapper
{
    public static ClientSecret ToClientEntity(this SecretApiDto model, int clientId)
    {
        if (model == null)
        {
            return null;
        }

        var entity = model.ToEntity<ClientSecret>();
        entity.ClientId = clientId;

        return entity;
    }
}
