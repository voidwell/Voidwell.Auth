using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.UserManagement.Models;

public class UserRolesRequest
{
    [Required]
    public IEnumerable<string> Roles { get; set; }
}
