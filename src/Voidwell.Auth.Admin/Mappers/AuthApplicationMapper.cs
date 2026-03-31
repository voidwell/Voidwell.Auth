using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using OpenIddict.Abstractions;
using Voidwell.Auth.Admin.Dtos;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Admin.Mappers;

internal static class AuthApplicationMapper
{
    public static AuthApplicationDto ToDto(this AuthApplication entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new AuthApplicationDto
        {
            Id = entity.Id,
            Enabled = entity.Enabled,
            ClientId = entity.ClientId,
            ClientName = entity.DisplayName,
            ClientType = entity.ClientType,
            Description = entity.Description,
            ClientUri = entity.ClientUri,
            LogoUri = entity.ClientLogoUri,
            ConsentType = entity.ConsentType,
            AllowRememberConsent = entity.AllowRememberConsent,
            AllowOfflineAccess = entity.AllowOfflineAccess,
            AccessTokenType = entity.AccessTokenType,
            EnableLocalLogin = entity.EnableLocalLogin,
            AllowedCorsOrigins = entity.AllowedCorsOrigins,
            PostLogoutRedirectUris = string.IsNullOrEmpty(entity.PostLogoutRedirectUris) ? [] : JsonSerializer.Deserialize<List<string>>(entity.PostLogoutRedirectUris),
            RedirectUris = string.IsNullOrEmpty(entity.RedirectUris) ? [] : JsonSerializer.Deserialize<List<string>>(entity.RedirectUris),
            FrontChannelLogoutUri = entity.FrontChannelLogoutUri,
            BackChannelLogoutUri = entity.BackChannelLogoutUri,
            Permissions = ToPermissionsModel(entity.Permissions),
            Settings = ToSettingsModel(entity.Settings)
        };
    }

    public static AuthApplication ToEntity(this AuthApplicationDto model)
    {
        if (model == null)
        {
            return null;
        }

        return new AuthApplication
        {
            Id = model.Id,
            Enabled = model.Enabled,
            ClientId = model.ClientId,
            DisplayName = model.ClientName,
            ClientType = model.ClientType,
            Description = model.Description,
            ClientUri = model.ClientUri,
            ClientLogoUri = model.LogoUri,
            ConsentType = model.ConsentType,
            AllowRememberConsent = model.AllowRememberConsent,
            AllowOfflineAccess = model.AllowOfflineAccess,
            AccessTokenType = model.AccessTokenType,
            EnableLocalLogin = model.EnableLocalLogin,
            AllowedCorsOrigins = model.AllowedCorsOrigins,
            RedirectUris = JsonSerializer.Serialize(model.RedirectUris),
            PostLogoutRedirectUris = JsonSerializer.Serialize(model.PostLogoutRedirectUris),
            FrontChannelLogoutUri = model.FrontChannelLogoutUri,
            BackChannelLogoutUri = model.BackChannelLogoutUri,
            Permissions = ToPermissionsEntity(model.Permissions),
            Settings = ToSettingsEntity(model.Settings)
        };
    }

    private static ClientApiPermissionsDto ToPermissionsModel(string permissionsJson)
    {
        if (string.IsNullOrEmpty(permissionsJson))
        {
            return new ClientApiPermissionsDto();
        }

        var permissions = JsonSerializer.Deserialize<HashSet<string>>(permissionsJson) ?? [];
        return new ClientApiPermissionsDto
        {
            AllowedGrantTypes = [.. permissions
                .Where(p => p.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType))
                .Select(p => p[OpenIddictConstants.Permissions.Prefixes.GrantType.Length..])],
            AllowedScopes = [.. permissions
                .Where(p => p.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope))
                .Select(p => p[OpenIddictConstants.Permissions.Prefixes.Scope.Length..])],
            AllowedResponseTypes = [.. permissions
                .Where(p => p.StartsWith(OpenIddictConstants.Permissions.Prefixes.ResponseType))
                .Select(p => p[OpenIddictConstants.Permissions.Prefixes.ResponseType.Length..])],
            AllowedEndpoints = [.. permissions
                .Where(p => p.StartsWith(OpenIddictConstants.Permissions.Prefixes.Endpoint))
                .Select(p => p[OpenIddictConstants.Permissions.Prefixes.Endpoint.Length..])]
        };
    }

    private static string ToPermissionsEntity(ClientApiPermissionsDto model)
    {
        var permissions = new HashSet<string>();

        model.AllowedGrantTypes?.ForEach(gt =>
            permissions.Add(OpenIddictConstants.Permissions.Prefixes.GrantType + gt));

        model.AllowedScopes?.ForEach(s =>
            permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + s));

        model.AllowedResponseTypes?.ForEach(rt =>
            permissions.Add(OpenIddictConstants.Permissions.Prefixes.ResponseType + rt));

        model.AllowedEndpoints?.ForEach(ep =>
            permissions.Add(OpenIddictConstants.Permissions.Prefixes.Endpoint + ep));

        return JsonSerializer.Serialize(permissions);
    }

    private static ClientApiSettingsDto ToSettingsModel(string settingsJson)
    {
        if (string.IsNullOrEmpty(settingsJson))
        {
            return new ClientApiSettingsDto();
        }

        var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(settingsJson);
        return new ClientApiSettingsDto
        {
            IdentityTokenLifetime = TryGetValue(settings, OpenIddictConstants.Settings.TokenLifetimes.IdentityToken),
            AccessTokenLifetime = TryGetValue(settings, OpenIddictConstants.Settings.TokenLifetimes.AccessToken),
            AuthorizationCodeLifetime = TryGetValue(settings, OpenIddictConstants.Settings.TokenLifetimes.AuthorizationCode),
            RefreshTokenLifetime = TryGetValue(settings, OpenIddictConstants.Settings.TokenLifetimes.RefreshToken),
            DeviceCodeLifetime = TryGetValue(settings, OpenIddictConstants.Settings.TokenLifetimes.DeviceCode)
        };
    }

    private static string ToSettingsEntity(ClientApiSettingsDto model)
    {
        var settings = new Dictionary<string, string>
        {
            { OpenIddictConstants.Settings.TokenLifetimes.IdentityToken, ToTimeSpanEntityValue(model.IdentityTokenLifetime) },
            { OpenIddictConstants.Settings.TokenLifetimes.AccessToken, ToTimeSpanEntityValue(model.AccessTokenLifetime) },
            { OpenIddictConstants.Settings.TokenLifetimes.AuthorizationCode, ToTimeSpanEntityValue(model.AuthorizationCodeLifetime) },
            { OpenIddictConstants.Settings.TokenLifetimes.RefreshToken, ToTimeSpanEntityValue(model.RefreshTokenLifetime) },
            { OpenIddictConstants.Settings.TokenLifetimes.DeviceCode, ToTimeSpanEntityValue(model.DeviceCodeLifetime) }
        };
        return JsonSerializer.Serialize(settings);
    }

    private static int TryGetValue(Dictionary<string, string> source, string key)
    {
        if (source.TryGetValue(key, out string value) && TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var typeValue))
        {
            return (int)typeValue.TotalSeconds;
        }
        return 0;
    }

    private static string ToTimeSpanEntityValue(int seconds)
    {
        return TimeSpan.FromSeconds(seconds).ToString("c", CultureInfo.InvariantCulture);
    }
}