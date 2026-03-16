using System;

namespace Voidwell.Auth.Admin.Dtos;

public class ClientSecretDto
{
    public int Id { get; set; }

    public string Description { get; set; }

    public string Value { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Expiration { get; set; }
}
