using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.IdentityServer.EntityMappings;

internal static class SecretMapper
{
    public static SecretApiDto ToModel(this Secret entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new SecretApiDto
        {
            Id = entity.Id,
            Created = entity.Created,
            Type = entity.Type,
            Description = entity.Description,
            Value = entity.Value,
            Expiration = entity.Expiration
        };
    }

    public static T ToEntity<T>(this SecretApiDto model)
        where T : Secret, new()
    {
        if (model == null)
        {
            return null;
        }

        var entity = new T
        {
            Created = model.Created,
            Type = model.Type,
            Description = model.Description,
            Value = model.Value,
            Expiration = model.Expiration
        };

        if (model.Id.HasValue)
        {
            entity.Id = model.Id.Value;
        }

        return entity;
    }
}
