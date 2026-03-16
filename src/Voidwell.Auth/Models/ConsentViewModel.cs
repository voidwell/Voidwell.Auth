using System.Collections.Generic;

namespace Voidwell.Auth.Models;

public class ConsentViewModel : ConsentInputModel
{
    public string ClientName { get; set; }
    public string ClientUrl { get; set; }
    public string ClientLogoUrl { get; set; }
    public bool AllowRememberConsent { get; set; }

    public IEnumerable<ScopeViewModel> Scopes { get; set; }
}
