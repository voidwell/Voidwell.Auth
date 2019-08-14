using System.Threading.Tasks;
using Voidwell.Auth.Models;

namespace Voidwell.Auth.Services
{
    public interface IAccountService
    {
        Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl);
        Task<LoginViewModel> BuildLoginViewModelAsync(AuthenticationRequest authRequest);
        Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId);
        Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId);
    }
}
