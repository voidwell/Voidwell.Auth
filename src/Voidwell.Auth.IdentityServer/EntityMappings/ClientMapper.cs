using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.IdentityServer.EntityMappings;

internal static class ClientMapper
{
    public static ClientApiDto ToModel(this Client entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new ClientApiDto
        {
            Id = entity.Id,
            ClientId = entity.ClientId,
            ClientName = entity.ClientName,
            Description = entity.Description,
            ProtocolType = entity.ProtocolType,
            RequireClientSecret = entity.RequireClientSecret,
            Enabled = entity.Enabled,
            RequireConsent = entity.RequireConsent,
            AllowRememberConsent = entity.AllowRememberConsent,
            AlwaysIncludeUserClaimsInIdToken = entity.AlwaysIncludeUserClaimsInIdToken,
            RequirePkce = entity.RequirePkce,
            AllowPlainTextPkce = entity.AllowPlainTextPkce,
            AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser,
            FrontChannelLogoutUri = entity.FrontChannelLogoutUri,
            FrontChannelLogoutSessionRequired = entity.FrontChannelLogoutSessionRequired,
            BackChannelLogoutUri = entity.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = entity.BackChannelLogoutSessionRequired,
            AllowOfflineAccess = entity.AllowOfflineAccess,
            IdentityTokenLifetime = entity.IdentityTokenLifetime,
            AccessTokenLifetime = entity.AccessTokenLifetime,
            AuthorizationCodeLifetime = entity.AuthorizationCodeLifetime,
            AbsoluteRefreshTokenLifetime = entity.AbsoluteRefreshTokenLifetime,
            SlidingRefreshTokenLifetime = entity.SlidingRefreshTokenLifetime,
            RefreshTokenUsage = entity.RefreshTokenUsage,
            UpdateAccessTokenClaimsOnRefresh = entity.UpdateAccessTokenClaimsOnRefresh,
            RefreshTokenExpiration = entity.RefreshTokenExpiration,
            AccessTokenType = entity.AccessTokenType,
            EnableLocalLogin = entity.EnableLocalLogin,
            IncludeJwtId = entity.IncludeJwtId,
            AlwaysSendClientClaims = entity.AlwaysSendClientClaims,
            ClientClaimsPrefix = entity.ClientClaimsPrefix,
            PairWiseSubjectSalt = entity.PairWiseSubjectSalt,
            UserSsoLifetime = entity.UserSsoLifetime,
            ConsentLifetime = entity.ConsentLifetime,
            ClientUri = entity.ClientUri,
            LogoUri = entity.LogoUri,
            UserCodeType = entity.UserCodeType,
            DeviceCodeLifetime = entity.DeviceCodeLifetime,
            AllowedScopes = entity.AllowedScopes?.Select(s => s.Scope).ToList() ?? [],
            PostLogoutRedirectUris = entity.PostLogoutRedirectUris?.Select(p => p.PostLogoutRedirectUri).ToList() ?? [],
            RedirectUris = entity.RedirectUris?.Select(r => r.RedirectUri).ToList() ?? [],
            IdentityProviderRestrictions = entity.IdentityProviderRestrictions?.Select(i => i.Provider).ToList() ?? [],
            AllowedCorsOrigins = entity.AllowedCorsOrigins?.Select(c => c.Origin).ToList() ?? [],
            AllowedGrantTypes = entity.AllowedGrantTypes?.Select(g => g.GrantType).ToList() ?? [],
            Claims = entity.Claims?.Select(c => new ClientClaimApiDto {
                Id = c.Id,
                Type = c.Type,
                Value = c.Value
            }).ToList() ?? [],
            Properties = entity.Properties?.Select(p => new ClientPropertyApiDto {
                Id = p.Id,
                Key = p.Key,
                Value = p.Value
            }).ToList() ?? []
        };
    }

    public static Client ToEntity(this ClientApiDto model)
    {
        if (model == null)
        {
            return null;
        }
        
        return new Client
        {
            Id = model.Id,
            ClientId = model.ClientId,
            ClientName = model.ClientName,
            Description = model.Description,
            ProtocolType = model.ProtocolType,
            RequireClientSecret = model.RequireClientSecret,
            Enabled = model.Enabled,
            RequireConsent = model.RequireConsent,
            AllowRememberConsent = model.AllowRememberConsent,
            AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
            RequirePkce = model.RequirePkce,
            AllowPlainTextPkce = model.AllowPlainTextPkce,
            AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,
            FrontChannelLogoutUri = model.FrontChannelLogoutUri,
            FrontChannelLogoutSessionRequired = model.FrontChannelLogoutSessionRequired,
            BackChannelLogoutUri = model.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = model.BackChannelLogoutSessionRequired,
            AllowOfflineAccess = model.AllowOfflineAccess,
            IdentityTokenLifetime = model.IdentityTokenLifetime,
            AccessTokenLifetime = model.AccessTokenLifetime,
            AuthorizationCodeLifetime = model.AuthorizationCodeLifetime,
            AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime,
            SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime,
            RefreshTokenUsage = model.RefreshTokenUsage,
            UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh,
            RefreshTokenExpiration = model.RefreshTokenExpiration,
            AccessTokenType = model.AccessTokenType,
            EnableLocalLogin = model.EnableLocalLogin,
            IncludeJwtId = model.IncludeJwtId,
            AlwaysSendClientClaims = model.AlwaysSendClientClaims,
            ClientClaimsPrefix = model.ClientClaimsPrefix,
            PairWiseSubjectSalt = model.PairWiseSubjectSalt,
            UserSsoLifetime = model.UserSsoLifetime,
            ConsentLifetime = model.ConsentLifetime,
            ClientUri = model.ClientUri,
            LogoUri = model.LogoUri,
            UserCodeType = model.UserCodeType,
            DeviceCodeLifetime = model.DeviceCodeLifetime,
            AllowedScopes = model.AllowedScopes?.Select(s => new ClientScope {
                Scope = s,
                ClientId = model.Id
            }).ToList() ?? [],
            RedirectUris = model.RedirectUris?.Select(r => new ClientRedirectUri {
                RedirectUri = r,
                ClientId = model.Id
            }).ToList() ?? [],
            PostLogoutRedirectUris = model.PostLogoutRedirectUris?.Select(p => new ClientPostLogoutRedirectUri {
                PostLogoutRedirectUri = p,
                ClientId = model.Id
            }).ToList() ?? [],
            IdentityProviderRestrictions = model.IdentityProviderRestrictions?.Select(i => new ClientIdPRestriction {
                Provider = i,
                ClientId = model.Id
            }).ToList() ?? [],
            AllowedCorsOrigins = model.AllowedCorsOrigins?.Select(o => new ClientCorsOrigin {
                Origin = o,
                ClientId = model.Id
            }).ToList() ?? [],
            AllowedGrantTypes = model.AllowedGrantTypes?.Select(g => new ClientGrantType {
                GrantType = g,
                ClientId = model.Id
            }).ToList() ?? [],
            Claims = model.Claims?.Select(c => new ClientClaim {
                Id = c.Id,
                Type = c.Type,
                Value = c.Value,
                ClientId = model.Id
            }).ToList() ?? [],
            Properties = model.Properties?.Select(p => new ClientProperty {
                Id = p.Id,
                Key = p.Key,
                Value = p.Value,
                ClientId = model.Id
            }).ToList() ?? []
        };
    }
}