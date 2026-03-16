using System.Threading.Tasks;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.UserManagement.Services.Abstractions;

public interface IRegistrationService
{
    Task<ApplicationUser> RegisterNewUserAsync(RegistrationForm registrationForm);
}
