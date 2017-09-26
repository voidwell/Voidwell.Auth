using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.VoidwellAuth.Data.Models
{
    public class User
    {
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Authentication Authentication { get; set; }
        [Required]
        public Profile Profile { get; set; }
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Banned { get; set; }
    }
}
