using System;
using System.Collections.Generic;
using System.Linq;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.Models
{
    public class LoginViewModel : AuthenticationRequest
    {
        public bool AllowRememberLogin { get; set; }
        public bool EnableLocalLogin { get; set; }

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => ExternalProviders?.SingleOrDefault()?.AuthenticationScheme;

        public string Error { get; set; }
    }
}
