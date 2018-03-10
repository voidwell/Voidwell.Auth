using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.Models
{
    public class SecretRequest
    {
        [Required]
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
