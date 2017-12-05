using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Voidwell.Auth.Models
{
    public class AuthenticationResult
    {
        public Guid UserId { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
