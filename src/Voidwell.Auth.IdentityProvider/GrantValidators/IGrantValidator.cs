using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Voidwell.Auth.IdentityProvider.Models;

namespace Voidwell.Auth.IdentityProvider.GrantValidators;

public interface IGrantValidator
{
    public string GrantType { get; }

    public Task<GrantValidationResult> ValidateAsync(OpenIddictRequest request);
}
