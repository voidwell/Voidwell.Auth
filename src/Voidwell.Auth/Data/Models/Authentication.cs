using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.Auth.Data.Models
{
    public class Authentication
    {
        [Key]
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string PasswordSalt { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTimeOffset? PasswordSetDate { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
