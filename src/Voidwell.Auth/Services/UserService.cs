using System;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using Microsoft.EntityFrameworkCore;
using Voidwell.Auth.Data;

namespace Voidwell.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly Func<AuthDbContext> _dbContextFactory;
        private readonly IUserCryptography _crypt;

        public UserService(Func<AuthDbContext> dbContextFactory, IUserCryptography crypt)
        {
            _dbContextFactory = dbContextFactory;
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

            var dbContext = _dbContextFactory();

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return user;
        }

        public Task<User> GetUser(Guid userId)
        {
            var dbContext = _dbContextFactory();

            return dbContext.Users
                .Include(user => user.Authentication)
                .Include(user => user.Profile)
                .SingleOrDefaultAsync(a => a.Id == userId);
        }

        public Task<User> GetUserByEmail(string email)
        {
            var dbContext = _dbContextFactory();

            return dbContext.Users
                .Include(user => user.Authentication)
                .Include(user => user.Profile)
                .SingleOrDefaultAsync(a => a.Profile.Email == email);
        }

        public async Task<User> UpdateUser(User user)
        {
            var dbContext = _dbContextFactory();

            var storageUser = await dbContext.Users.SingleOrDefaultAsync(a => a.Id == user.Id);

            storageUser.Profile = user.Profile;

            dbContext.Update(user);
            await dbContext.SaveChangesAsync();

            return storageUser;
        }
    }
}
