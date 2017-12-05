using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voidwell.Auth.Quickstart.Account;

namespace Voidwell.VoidwellAuth.Client
{
    public class PasswordResetViewModel
    {
        [Required]
        public string Username { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<SecurityQuestion> SecurityQuestions { get; set; }
    }
}
