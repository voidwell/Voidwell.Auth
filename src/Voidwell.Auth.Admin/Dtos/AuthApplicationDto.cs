using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.Admin.Dtos;

public class AuthApplicationDto
{
    public int Id { get; set; }

    public bool Enabled { get; set; } = true;

    [Required]
    public string ClientId { get; set; }

    [Required]
    public string ClientName { get; set; }

    /// <summary>
    /// "public" or "confidential"
    /// </summary>
    public string ClientType { get; set; } = "confidential";

    public string Description { get; set; }

    public string ClientUri { get; set; }

    public string LogoUri { get; set; }

    /// <summary>
    /// e.g. "explicit", "implicit", "external"
    /// </summary>
    public string ConsentType { get; set; } = "explicit";

    public bool AllowRememberConsent { get; set; } = true;

    public bool AllowOfflineAccess { get; set; } = false;

    /// <summary>
    /// e.g. "jwt" or "reference"
    /// </summary>
    public string AccessTokenType { get; set; } = "jwt";

    public bool EnableLocalLogin { get; set; } = true;

    public List<string> AllowedCorsOrigins { get; set; } = [];

    public List<string> PostLogoutRedirectUris { get; set; } = [];

    public List<string> RedirectUris { get; set; } = [];

    /// <summary>
    /// Client permissions (grant types, scopes, response types/modes, token auth methods)
    /// </summary>
    public ClientApiPermissionsDto Permissions { get; set; } = new ClientApiPermissionsDto();

    /// <summary>
    /// Token/refresh/client settings (lifetimes, PKCE, refresh token behavior)
    /// </summary>
    public ClientApiSettingsDto Settings { get; set; } = new ClientApiSettingsDto();

    /// <summary>
    /// Arbitrary properties bag to preserve any additional OpenIddict properties
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = [];
}

public class ClientApiPermissionsDto
{
    /// <summary>
    /// grant types like "authorization_code", "client_credentials", "refresh_token", "implicit", "password"
    /// </summary>
    public List<string> AllowedGrantTypes { get; set; } = [];

    /// <summary>
    /// scopes allowed (e.g., "openid", "profile", "email", "api1")
    /// </summary>
    public List<string> AllowedScopes { get; set; } = [];

    /// <summary>
    /// response types like "code", "token", "id_token"
    /// </summary>
    public List<string> AllowedResponseTypes { get; set; } = [];

    /// <summary>
    /// client authentication methods for token endpoint, e.g. "client_secret_basic", "client_secret_post", "none"
    /// </summary>
    public List<string> TokenEndpointAuthMethods { get; set; } = [];

    /// <summary>
    /// Allowed endpoints like "authorization", "token", "logout", "revocation", "introspection", "device"
    /// </summary>
    public List<string> AllowedEndpoints { get; set; } = [];

    /// <summary>
    /// Allow access tokens to be returned in the browser (implicit-flow related)
    /// </summary>
    public bool AllowAccessTokensViaBrowser { get; set; } = false;
}

public class ClientApiSettingsDto
{
    // standard token lifetimes (seconds)
    public int IdentityTokenLifetime { get; set; } = 300;
    public int AccessTokenLifetime { get; set; } = 3600;
    public int AuthorizationCodeLifetime { get; set; } = 300;
    public int RefreshTokenLifetime { get; set; } = 2592000;
    public int DeviceCodeLifetime { get; set; } = 300;

    // refresh token behaviours
    /// <summary>
    /// "ReUse" or "OneTimeOnly"
    /// </summary>
    public string RefreshTokenUsage { get; set; } = "ReUse";

    /// <summary>
    /// "Absolute" or "Sliding"
    /// </summary>
    public string RefreshTokenExpiration { get; set; } = "Absolute";

    /// <summary>
    /// Sliding lifetime; used when RefreshTokenExpiration == "Sliding"
    /// </summary>
    public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

    // client requirements/settings
    public bool RequireProofKeyForCodeExchange { get; set; } = true;
    public bool RequireClientSecret { get; set; } = true;
}
