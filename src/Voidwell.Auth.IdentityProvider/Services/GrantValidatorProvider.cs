using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.Auth.IdentityProvider.GrantValidators;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider.Services;

internal class GrantValidatorProvider : IGrantValidatorProvider
{
    public IServiceProvider _serviceProvider;

    public GrantValidatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IGrantValidator GetValidator(string grantType)
    {
        var validators = _serviceProvider.GetServices<IGrantValidator>();
        return validators.FirstOrDefault(v => v.GrantType == grantType);
    }
}
