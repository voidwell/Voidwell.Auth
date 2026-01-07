using Voidwell.Auth.IdentityProvider.Models;

namespace Voidwell.Auth.Admin.Models;

public class CreatedSecretResponse
{
    public CreatedSecretResponse(string value, SecretApiDto dto)
    {
        Value = value;
        Model = dto;
    }

    public string Value { get; set; }
    public SecretApiDto Model { get; set; }
}
