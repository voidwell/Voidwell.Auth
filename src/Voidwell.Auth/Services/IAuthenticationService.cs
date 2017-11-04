using System.Threading.Tasks;
using Voidwell.Auth.Models;

namespace Voidwell.Auth.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Authenticate(string username, string password);
    }
}
