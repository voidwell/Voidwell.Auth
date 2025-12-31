using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.Admin.Models;

public class SecretApiDto
{
    [Required]
    public string Type { get; set; } = "SharedSecret";

    public int Id { get; set; }

    public string Description { get; set; }

    [Required]
    public string Value { get; set; }

    public DateTime? Expiration { get; set; }
}
