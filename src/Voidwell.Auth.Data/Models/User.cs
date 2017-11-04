using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.Data.Models
{
    public class User
    {
        [Key]
        [Required]
        public Guid Id { get; set; }        
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Banned { get; set; }

        public Authentication Authentication { get; set; }
        public Profile Profile { get; set; }
        public IEnumerable<SecurityQuestion> SecurityQuestions { get; set; }
    }
}
