using System.ComponentModel.DataAnnotations;

namespace Voidwell.VoidwellAuthMVCClient.Models
{
    public class RegistrationForm
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
