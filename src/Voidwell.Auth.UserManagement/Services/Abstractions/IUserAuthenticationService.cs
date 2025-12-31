using System.Threading.Tasks;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.UserManagement.Services.Abstractions;

public interface IUserAuthenticationService
{
    Task<AuthenticationResult> Authenticate(AuthenticationRequest authRequest);
}
