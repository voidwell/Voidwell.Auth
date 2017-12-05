using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace Voidwell.Auth.Delegation
{
    public class DelegationTokenValidationService : IDelegationTokenValidationService
    {
        private readonly ITokenValidator _validator;

        public DelegationTokenValidationService(ITokenValidator validator)
        {
            _validator = validator;
        }

        public Task<TokenValidationResult> GetTokenValidationResultAsync(string userToken)
        {
            return _validator.ValidateAccessTokenAsync(userToken);
        }
    }
}
