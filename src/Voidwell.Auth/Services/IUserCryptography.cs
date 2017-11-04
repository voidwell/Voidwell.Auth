namespace Voidwell.Auth.Services
{
    public interface IUserCryptography
    {
        string GenerateSalt();
        string GenerateHash(string input, string salt);
    }
}
