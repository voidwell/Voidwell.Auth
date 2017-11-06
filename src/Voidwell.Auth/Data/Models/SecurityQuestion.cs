using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.Auth.Data.Models
{
    public class SecurityQuestion
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string Answer { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
