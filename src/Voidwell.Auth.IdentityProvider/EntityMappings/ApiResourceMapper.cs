using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.IdentityProvider.Models;

namespace Voidwell.Auth.IdentityProvider.EntityMappings;

internal static class ApiResourceMapper
{
    public static ApiResourceApiDto ToModel(this ApiResource entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new ApiResourceApiDto
        {
            Id = entity.Id,
            Name = entity.Name,
            DisplayName = entity.DisplayName,
            Description = entity.Description,
            Enabled = entity.Enabled,
            UserClaims = entity.UserClaims?.Select(c => c.Type).ToList() ?? [],
            Properties = entity.Properties?.Select(p => new ApiResourcePropertyApiDto {
                Id = p.Id,
                Key = p.Key,
                Value = p.Value
            }).ToList() ?? [],
            Scopes = entity.Scopes?.Select(s => new ApiScopeApiDto {
                Id = s.Id,
                Name = s.Name,
                DisplayName = s.DisplayName,
                Description = s.Description,
                Required = s.Required,
                Emphasize = s.Emphasize,
                UserClaims = s.UserClaims?.Select(uc => uc.Type).ToList() ?? []
            }).ToList() ?? []
        };
    }

    public static ApiResource ToEntity(this ApiResourceApiDto model)
    {
        if (model == null)
        {
            return null;
        }

        return new ApiResource
        {
            Id = model.Id,
            Name = model.Name,
            DisplayName = model.DisplayName,
            Description = model.Description,
            Enabled = model.Enabled,
            UserClaims = model.UserClaims?.Select(c => new ApiResourceClaim {
                Type = c,
                ApiResourceId = model.Id
            }).ToList() ?? [],
            Properties = model.Properties?.Select(p => new ApiResourceProperty {
                Id = p.Id,
                Key = p.Key,
                Value = p.Value,
                ApiResourceId = model.Id
            }).ToList() ?? [],
            Scopes = model.Scopes?.Select(s => new ApiScope
            {
                Id = s.Id,
                Name = s.Name,
                DisplayName = s.DisplayName,
                Description = s.Description,
                Required = s.Required,
                Emphasize = s.Emphasize,
                UserClaims = s.UserClaims?.Select(uc => new ApiScopeClaim {
                    Type = uc,
                    ApiScopeId = s.Id
                }).ToList() ?? [],
                ApiResourceId = model.Id
            }).ToList() ?? []
        };
    }
}
