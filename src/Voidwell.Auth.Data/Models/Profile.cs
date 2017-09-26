using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.VoidwellAuth.Data.Models
{
    public class Profile
    {
        public Guid ProfileId { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Email { get; set; }
        public string TimeZone { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
