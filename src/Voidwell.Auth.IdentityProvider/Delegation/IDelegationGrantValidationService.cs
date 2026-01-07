using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace Voidwell.Auth.IdentityProvider.Delegation;

public interface IDelegationGrantValidationService
{
    Task<GrantValidationResult> GetGrantValidationResultAsync(string userToken);
}
