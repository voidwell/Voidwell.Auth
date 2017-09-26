namespace Voidwell.VoidwellAuth.IdentityServer.Services
{
    public interface IUserCryptography
    {
        string GenerateSalt();
        string GenerateHash(string input, string salt);
    }
}
