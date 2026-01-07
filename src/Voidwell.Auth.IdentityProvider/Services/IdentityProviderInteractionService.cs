using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider.Services;

public class IdentityProviderInteractionService : IIdentityProviderInteractionService
{
    private readonly IIdentityServerInteractionService _identityServerInteractionService;

    public IdentityProviderInteractionService(IIdentityServerInteractionService identityServerInteractionService)
    {
        _identityServerInteractionService = identityServerInteractionService;
    }

    public bool IsValidReturnUrl(string returnUrl)
    {
        return _identityServerInteractionService.IsValidReturnUrl(returnUrl);
    }

    public Task<AuthorizationRequest> GetAuthorizationContextAsync(string returnUrl)
    {
        return _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
    }

    public Task<string> CreateLogoutContextAsync()
    {
        return _identityServerInteractionService.CreateLogoutContextAsync();
    }

    public Task<LogoutRequest> GetLogoutContextAsync(string logoutId)
    {
        return _identityServerInteractionService.GetLogoutContextAsync(logoutId);
    }

    public Task GrantConsentAsync(AuthorizationRequest request, ConsentResponse grantedConsent)
    {
        return _identityServerInteractionService.GrantConsentAsync(request, grantedConsent);
    }
}