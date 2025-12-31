using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.UserManagement.Models;

public class RoleRequest
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
}
