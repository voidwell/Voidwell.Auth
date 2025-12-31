using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace Voidwell.Auth.IdentityServer.Delegation;

public interface IDelegationGrantValidationService
{
    Task<GrantValidationResult> GetGrantValidationResultAsync(string userToken);
}
