using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Models;

namespace Voidwell.Auth.Services.Abstractions;

public interface IConsentService
{
    IEnumerable<string> GetConsentedScopes(ConsentInputModel model);

    Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null);
}
