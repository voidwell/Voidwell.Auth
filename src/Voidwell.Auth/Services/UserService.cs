using System;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using Microsoft.EntityFrameworkCore;
using Voidwell.Auth.Data;
using Microsoft.AspNetCore.Identity;
using Voidwell.Auth.Models;
using Microsoft.Extensions.Logging;

namespace Voidwell.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserCryptography _crypt;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, IUserCryptography crypt, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _crypt = crypt;
            _logger = logger;
        }

        public async Task<ApplicationUser> CreateUser(string displayName, string email, string password)
        {
            var salt = _crypt.GenerateSalt();
            var hash = _crypt.GenerateHash(password, salt);

            var user = new User
            {
                Authentication = new Authentication
                {
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    LastLoginDate = DateTimeOffset.Now,
                    PasswordSetDate = DateTimeOffset.Now
                },
                Profile = new Profile
                {
                    Email = email,
                    DisplayName = displayName
                },
                Created = DateTimeOffset.Now
            };

            var newUser = new ApplicationUser
            {
                UserName = displayName,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                LastLoginDate = DateTimeOffset.Now,
                PasswordSetDate = DateTimeOffset.Now,
                CreatedDate = DateTimeOffset.Now
            };

            var result = await _userManager.CreateAsync(newUser);

            return newUser;
        }

        public Task<ApplicationUser> GetUser(Guid userId)
        {
            return _userManager.Users.SingleOrDefaultAsync(a => a.Id == userId);
        }

        public Task<ApplicationUser> GetUserByEmail(string email)
        {
            return _userManager.Users.SingleOrDefaultAsync(a => a.Email == email);
        }

        public async Task<ApplicationUser> UpdateUser(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
            return user;
        }
    }
}
