using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.Auth.Data.Models
{
    public class Profile
    {
        [Key]
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Email { get; set; }
        public string TimeZone { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
