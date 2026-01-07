using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace Voidwell.Auth.IdentityProvider.Delegation;

public interface IDelegationTokenValidationService
{
    Task<TokenValidationResult> GetTokenValidationResultAsync(string userToken);
}