using System.Collections.Generic;

namespace Voidwell.Auth.Models;

public class VerificationViewModel
{
    public string UserCode { get; set; }
    public string ApplicationName { get; set; }
    public IEnumerable<string> Scopes { get; set; }
}
