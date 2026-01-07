using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.IdentityServer.Models;

public class IdentityResourceDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; }

    public List<string> UserClaims { get; set; }

    public List<ApiResourcePropertyApiDto> Properties { get; set; }

    public DateTime Created { get; set; }
}
