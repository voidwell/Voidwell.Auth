using System.Collections.Generic;
using OpenIddict.Abstractions;

namespace Voidwell.Auth.Data.Models;

public class AuthApplicationDescriptor : OpenIddictApplicationDescriptor
{
    public bool Enabled { get; set; } = true;

    public string Description { get; set; }

    public string ClientUri { get; set; }

    public string ClientLogoUri { get; set; }

    public bool AllowRememberConsent { get; set; } = true;

    public bool AllowOfflineAccess { get; set; } = true;

    // "jwt" or "reference"
    public string AccessTokenType { get; set; } = "jwt";

    public bool EnableLocalLogin { get; set; } = true;

    public List<string> AllowedCorsOrigins { get; set; } = [];
}
