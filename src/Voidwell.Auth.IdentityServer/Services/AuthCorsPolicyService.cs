using IdentityServer4.Services;
using System.Threading.Tasks;

namespace Voidwell.Auth.IdentityServer.Services;

public class AuthCorsPolicyService : ICorsPolicyService
{
    public Task<bool> IsOriginAllowedAsync(string origin)
    {
        return Task.FromResult(true);
    }
}
