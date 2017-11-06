using System;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.Models;

namespace Voidwell.Auth.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> CreateUser(string displayName, string email, string password);
        Task<ApplicationUser> GetUser(Guid userId);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<ApplicationUser> UpdateUser(ApplicationUser user);
    }
}
