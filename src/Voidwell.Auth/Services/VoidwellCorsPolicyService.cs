using IdentityServer4.Services;
using System.Threading.Tasks;

namespace Voidwell.Auth.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(true);
        }
    }
}
