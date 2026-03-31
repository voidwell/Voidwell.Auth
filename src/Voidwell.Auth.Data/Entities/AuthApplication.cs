using System.Collections.Generic;
using OpenIddict.EntityFrameworkCore.Models;

namespace Voidwell.Auth.Data.Entities;

public class AuthApplication : OpenIddictEntityFrameworkCoreApplication<int, AuthAuthorization, AuthToken>
{
    public bool Enabled { get; set; } = true;

    public string Description { get; set; }

    public string ClientUri { get; set; }

    public string ClientLogoUri { get; set; }

    public bool AllowRememberConsent { get; set; } = true;

    public bool AllowOfflineAccess { get; set; } = true;

    public string AccessTokenType { get; set; } = "jwt";

    public bool EnableLocalLogin { get; set; } = true;

    public List<string> AllowedCorsOrigins { get; set; } = [];

    public string FrontChannelLogoutUri { get; set; }

    public string BackChannelLogoutUri { get; set; }
}
