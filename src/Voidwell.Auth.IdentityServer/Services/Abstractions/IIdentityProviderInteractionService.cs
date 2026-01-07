using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Voidwell.Auth.IdentityServer.Services.Abstractions;

public interface IIdentityProviderInteractionService
{
    bool IsValidReturnUrl(string returnUrl);

    Task<AuthorizationRequest> GetAuthorizationContextAsync(string returnUrl);

    Task<string> CreateLogoutContextAsync();

    Task<LogoutRequest> GetLogoutContextAsync(string logoutId);

    Task GrantConsentAsync(AuthorizationRequest request, ConsentResponse grantedConsent);
}