using IdentityModel;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Voidwell.Auth.Exceptions;
using Voidwell.Auth.Models;

namespace Voidwell.Auth.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IUserCryptography _crypt;
        private readonly ILogger _logger;

        public AuthenticationService(IUserService userService, IUserCryptography crypt, ILogger<AuthenticationService> logger)
        {
            _userService = userService;
            _crypt = crypt;
            _logger = logger;
        }

        public async Task<AuthenticationResult> Authenticate(string username, string password)
        {
            var user = await _userService.GetUserByEmail(username);

            _logger.LogDebug($"Attempting to login user: {username}");

            if (user == null)
            {
                _logger.LogWarning($"Failed to find user: {username}");
                return null;
            }

            var inputHash = _crypt.GenerateHash(password, user.Authentication.PasswordSalt);

            if (inputHash != user.Authentication.PasswordHash)
            {
                _logger.LogWarning($"Failed to authenticate for user: {username}");
                throw new InvalidPasswordException();
            }

            _logger.LogWarning($"Successfully authenticated user: {username}");

            return new AuthenticationResult
            {
                UserId = user.Id,
                Profile = user.Profile,
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                    new Claim(JwtClaimTypes.Email, user.Profile.Email),
                    new Claim(JwtClaimTypes.Name, user.Profile.DisplayName)
                }
            };
        }
    }
}
