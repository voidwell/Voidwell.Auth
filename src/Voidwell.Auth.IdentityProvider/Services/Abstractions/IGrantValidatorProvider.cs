using Voidwell.Auth.IdentityProvider.GrantValidators;

namespace Voidwell.Auth.IdentityProvider.Services.Abstractions;

public interface IGrantValidatorProvider
{
    public IGrantValidator GetValidator(string grantType);
}