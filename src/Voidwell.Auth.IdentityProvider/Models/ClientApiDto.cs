using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.IdentityProvider.Models;

public class ClientApiDto
{
    public ClientApiDto()
    {
        AllowedScopes = [];
        PostLogoutRedirectUris = [];
        RedirectUris = [];
        IdentityProviderRestrictions = [];
        AllowedCorsOrigins = [];
        AllowedGrantTypes = [];
        Claims = [];
        Properties = [];
    }

    public int Id { get; set; }
    public string ProtocolType { get; set; } = "oidc";
    public string Description { get; set; }

    // Basics
    public bool Enabled { get; set; } = true;
    [Required]
    public string ClientId { get; set; }
    [Required]
    public string ClientName { get; set; }
    public bool RequireClientSecret { get; set; } = true;
    public List<string> AllowedGrantTypes { get; set; }
    public bool RequirePkce { get; set; }
    public bool AllowPlainTextPkce { get; set; }
    public List<string> RedirectUris { get; set; }
    public List<string> AllowedScopes { get; set; }
    public bool AllowOfflineAccess { get; set; }
    public bool AllowAccessTokensViaBrowser { get; set; }
    public List<ClientPropertyApiDto> Properties { get; set; }

    // Authentication/Logout
    public List<string> PostLogoutRedirectUris { get; set; }
    public string FrontChannelLogoutUri { get; set; }
    public bool FrontChannelLogoutSessionRequired { get; set; } = true;
    public string BackChannelLogoutUri { get; set; }
    public bool BackChannelLogoutSessionRequired { get; set; } = true;
    public bool EnableLocalLogin { get; set; } = true;
    public List<string> IdentityProviderRestrictions { get; set; }
    public int? UserSsoLifetime { get; set; }

    // Token
    public int IdentityTokenLifetime { get; set; } = 300;
    public int AccessTokenLifetime { get; set; } = 3600;
    public int AuthorizationCodeLifetime { get; set; } = 300;
    public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
    public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
    public int RefreshTokenUsage { get; set; } = 1;
    public int RefreshTokenExpiration { get; set; } = 1;
    public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
    public int AccessTokenType { get; set; }
    public bool IncludeJwtId { get; set; }
    public List<string> AllowedCorsOrigins { get; set; }
    public List<ClientClaimApiDto> Claims { get; set; }
    public bool AlwaysSendClientClaims { get; set; }
    public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
    public string ClientClaimsPrefix { get; set; } = "client_";
    public string PairWiseSubjectSalt { get; set; }

    // Consent
    public bool RequireConsent { get; set; } = true;
    public bool AllowRememberConsent { get; set; } = true;
    public int? ConsentLifetime { get; set; }
    public string ClientUri { get; set; }
    public string LogoUri { get; set; }

    // Device Flow
    public string UserCodeType { get; set; }
    public int DeviceCodeLifetime { get; set; } = 300;
}
