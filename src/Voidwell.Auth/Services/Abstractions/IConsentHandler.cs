using IdentityServer4.Models;
using System.Threading.Tasks;
using Voidwell.Auth.Models;

namespace Voidwell.Auth.Services.Abstractions
{
    public interface IConsentHandler
    {
        Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model);

        Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null);

        ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check);

        ScopeViewModel CreateScopeViewModel(Scope scope, bool check);
    }
}
