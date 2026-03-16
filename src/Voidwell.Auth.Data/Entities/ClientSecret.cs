using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.Data.Entities;

public class ClientSecret
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    public DateTime Created { get; set; }

    [MaxLength(2000)]
    public string Description { get; set; }

    public DateTime? Expiration { get; set; }

    [Required]
    [MaxLength(4000)]
    public string Value { get; set; }
}
