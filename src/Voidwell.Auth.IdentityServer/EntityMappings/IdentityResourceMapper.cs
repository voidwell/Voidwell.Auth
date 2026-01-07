using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.IdentityServer.EntityMappings;

internal static class IdentityResourceMapper
{
    public static IdentityResourceDto ToModel(this IdentityResource entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new IdentityResourceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            DisplayName = entity.DisplayName,
            Description = entity.Description,
            Required = entity.Required,
            Emphasize = entity.Emphasize,
            ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument,
            UserClaims = entity.UserClaims?.Select(c => c.Type).ToList() ?? [],
            Properties = entity.Properties?.Select(p => new ApiResourcePropertyApiDto {
                Id = p.Id,
                Key = p.Key,
                Value = p.Value
            }).ToList() ?? [],
            Created = entity.Created
        };
    }

    public static IdentityResource ToEntity(this IdentityResourceDto model)
    {
        if (model == null)
        {
            return null;
        }

        var entity = new IdentityResource
        {
            Id = model.Id,
            Name = model.Name,
            DisplayName = model.DisplayName,
            Description = model.Description,
            Required = model.Required,
            Emphasize = model.Emphasize,
            ShowInDiscoveryDocument = model.ShowInDiscoveryDocument,
            Created = model.Created,
            UserClaims = model.UserClaims?.Select(c => new IdentityClaim {
                Type = c,
                IdentityResourceId = model.Id
            }).ToList() ?? [],
            Properties = model.Properties?.Select(p => new IdentityResourceProperty {
                Id = p.Id,
                Key = p.Key,
                Value = p.Value,
                IdentityResourceId = model.Id
            }).ToList() ?? []
        };

        return entity;
    }
}
