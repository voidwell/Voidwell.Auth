using System;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(string displayName, string email, string password);
        Task<User> GetUser(Guid userId);
        Task<User> GetUserByEmail(string email);
        Task<User> UpdateUser(User user);
    }
}
