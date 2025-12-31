using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Voidwell.Auth.IdentityServer.Delegation;

public  class DelegationGrantValidator : IExtensionGrantValidator
{
    private readonly IDelegationGrantValidationService _delegationGrantValidationService;
    private readonly ILogger _logger;

    public DelegationGrantValidator(IDelegationGrantValidationService delegationGrantValidationService,
        ILogger<DelegationGrantValidator> logger)
    {
        _delegationGrantValidationService = delegationGrantValidationService;
        _logger = logger;
    }

    public string GrantType => "delegation";

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var userToken = context.Request.Raw.Get("token");

        if (string.IsNullOrEmpty(userToken))
        {
            _logger.LogWarning($"Null token passed to {nameof(DelegationGrantValidator)}");
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            return;
        }

        context.Result = await _delegationGrantValidationService.GetGrantValidationResultAsync(userToken);
    }
}