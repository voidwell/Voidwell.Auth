using System;
using System.Collections.Generic;
using System.Security.Claims;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Models
{
    public class AuthenticationResult
    {
        public Guid UserId { get; set; }
        public Profile Profile { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
