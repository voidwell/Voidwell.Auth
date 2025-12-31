using System.Threading.Tasks;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<string> Authenticate(AuthenticationRequest authRequest);
    }
}