using System;
using System.Security.Cryptography;
using System.Text;

namespace Voidwell.VoidwellAuth.IdentityServer.Services
{
    public class UserCryptography : IUserCryptography, IDisposable
    {
        private readonly RandomNumberGenerator _rng;

        public UserCryptography()
        {
            _rng = RandomNumberGenerator.Create();
        }

        public string GenerateSalt()
        {
            var buffer = new byte[16];
            _rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public string GenerateHash(string input, string salt)
        {
            byte[] bInput = Encoding.UTF8.GetBytes(input);
            byte[] bSalt = Encoding.UTF8.GetBytes(salt);

            var PBKDF2 = new Rfc2898DeriveBytes(bInput, bSalt, 1000);
            var bytes = PBKDF2.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        public void Dispose()
        {
            _rng.Dispose();
        }
    }
}
