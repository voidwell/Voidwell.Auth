using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.IdentityServer.Models;

public class ClientClaimApiDto
{
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Value { get; set; }
}