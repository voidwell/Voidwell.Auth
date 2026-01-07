using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.Auth.IdentityProvider.Models;

public class ApiResourceApiDto
{
    public ApiResourceApiDto()
    {
        UserClaims = [];
        Properties = [];
        Scopes = [];
    }

    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; } = true;

    public List<string> UserClaims { get; set; }

    public List<ApiResourcePropertyApiDto> Properties { get; set; }

    public List<ApiScopeApiDto> Scopes { get; set; }
}
