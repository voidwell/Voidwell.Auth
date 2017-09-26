using Voidwell.VoidwellAuth.IdentityServer.Models;

namespace Voidwell.VoidwellAuth.IdentityServer.Services
{
    public interface IAuthenticationService
    {
        AuthenticationResult Authenticate(string username, string password);
    }
}
