using System;
using System.Threading.Tasks;
using Voidwell.VoidwellAuth.Data.Models;

namespace Voidwell.VoidwellAuth.IdentityServer.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(string displayName, string email, string password);
        User GetUser(Guid userId);
        User GetUserByEmail(string email);
        Task<User> UpdateUser(User user);
    }
}
