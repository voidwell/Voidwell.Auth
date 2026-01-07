using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.IdentityServer.EntityMappings;

internal static class ApiSecretMapper
{
    public static ApiSecret ToApiEntity(this SecretApiDto model, int apiResourceId)
    {
        if (model == null)
        {
            return null;
        }

        var entity = model.ToEntity<ApiSecret>();
        entity.ApiResourceId = apiResourceId;

        return entity;
    }
}
