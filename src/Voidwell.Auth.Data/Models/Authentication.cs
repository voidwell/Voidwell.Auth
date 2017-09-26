using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.VoidwellAuth.Data.Models
{
    public class Authentication
    {
        public Guid AuthenticationId { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string PasswordSalt { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTimeOffset? PasswordSetDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
