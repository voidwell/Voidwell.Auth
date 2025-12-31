using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.UserManagement.Models;

public class ResetPasswordStart
{
    [Required]
    public string Email { get; set; }
}
