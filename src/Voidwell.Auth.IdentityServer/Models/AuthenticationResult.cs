using System;
using System.Collections.Generic;
using System.Security.Claims;
using Voidwell.VoidwellAuth.Data.Models;

namespace Voidwell.VoidwellAuth.IdentityServer.Models
{
    public class AuthenticationResult
    {
        public Guid UserId { get; set; }
        public Profile Profile { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
