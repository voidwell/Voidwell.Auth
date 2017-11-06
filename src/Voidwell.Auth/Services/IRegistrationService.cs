using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Services
{
    public interface IRegistrationService
    {
        Task<ApplicationUser> RegisterNewUserAsync(RegistrationForm registrationForm);
    }
}
