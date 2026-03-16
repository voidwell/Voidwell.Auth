namespace Voidwell.Auth.IdentityProvider.Models;

public class CreatedSecretResponse
{
    public CreatedSecretResponse(int id, string secretValue)
    {
        Id = id;
        Secret = secretValue;
    }

    public int Id { get; set; }
    
    public string Secret { get; set; }
}
