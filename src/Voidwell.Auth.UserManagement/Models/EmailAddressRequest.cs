using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.UserManagement.Models;

public class EmailAddressRequest
{
    [Required]
    public string EmailAddress { get; set; }
}
