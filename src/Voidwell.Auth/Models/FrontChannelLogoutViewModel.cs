using System.Collections.Generic;

namespace Voidwell.Auth.Models;

public class FrontChannelLogoutViewModel
{
    public IReadOnlyList<string> FrontChannelLogoutUris { get; set; }

    public string PostLogoutRedirectUri { get; set; }
}
