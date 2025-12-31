using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.UserManagement.Models;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string NewPassword { get; set; }
}
