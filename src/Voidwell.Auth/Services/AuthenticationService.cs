using IdentityModel;
using Microsoft.AspNetCore.Identity;
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
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly IUserService _userService;
        private readonly IUserCryptography _crypt;
        private readonly ILogger _logger;

        public AuthenticationService(SignInManager<ApplicationUser> signinManager, IUserService userService, IUserCryptography crypt, ILogger<AuthenticationService> logger)
        {
            _signinManager = signinManager;
            _userService = userService;
            _crypt = crypt;
            _logger = logger;
        }

        public async Task Authenticate(string username, string password)
        {
            var user = await _userService.GetUserByEmail(username);

            _logger.LogDebug($"Attempting to login user: {username}");

            if (user == null)
            {
                _logger.LogWarning($"Failed to find user: {username}");
                return;
            }

            var inputHash = _crypt.GenerateHash(password, user.PasswordSalt);

            if (inputHash != user.PasswordHash)
            {
                _logger.LogWarning($"Failed to authenticate for user: {username}");
                throw new InvalidPasswordException();
            }

            _logger.LogDebug($"Successfully authenticated user: {username}");

            await _signinManager.SignInAsync(user, true);
        }
    }
}
