using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.IdentityServer.EntityMappings;

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
