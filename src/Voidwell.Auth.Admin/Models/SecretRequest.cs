using System;

namespace Voidwell.Auth.Admin.Models
{
    public class SecretRequest
    {
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
