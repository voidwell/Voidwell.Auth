using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.VoidwellAuth.Data.Models;
using Voidwell.VoidwellAuth.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Voidwell.VoidwellAuth.IdentityServer.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _userContext;
        private readonly IUserCryptography _crypt;

        public UserService(UserDbContext userContext, IUserCryptography crypt)
        {
            _userContext = userContext;
            _crypt = crypt;
        }

        public async Task<User> CreateUser(string displayName, string email, string password)
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

            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();

            return user;
        }

        public User GetUser(Guid userId)
        {
            return _userContext.Users
                .Include(user => user.Authentication)
                .Include(user => user.Profile)
                .SingleOrDefault(a => a.Id == userId);
        }

        public User GetUserByEmail(string email)
        {
            return _userContext.Users
                .Include(user => user.Authentication)
                .Include(user => user.Profile)
                .SingleOrDefault(a => a.Profile.Email == email);
        }

        public async Task<User> UpdateUser(User user)
        {
            var storageUser = _userContext.Users.SingleOrDefault(a => a.Id == user.Id);

            storageUser.Profile = user.Profile;

            _userContext.Update(user);
            await _userContext.SaveChangesAsync();

            return storageUser;
        }
    }
}
