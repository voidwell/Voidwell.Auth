using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.IdentityServer.Models;
using Voidwell.Auth.IdentityServer.Services.Abstractions;
using Voidwell.Auth.Models;
using IConsentService = Voidwell.Auth.Services.Abstractions.IConsentService;

namespace Voidwell.Auth.Services;

public class ConsentService : IConsentService
{
    private readonly IIdentityProviderInteractionService _interaction;
    private readonly IIdentityProviderManager _idpm;
    private readonly ILogger<ConsentService> _logger;

    public ConsentService(IIdentityProviderInteractionService interaction, IIdentityProviderManager idpm, ILogger<ConsentService> logger)
    {
        _interaction = interaction;
        _idpm = idpm;
        _logger = logger;
    }

    public async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
    {
        var result = new ProcessConsentResult();

        ConsentResponse grantedConsent = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (model.Button == "no")
        {
            grantedConsent = ConsentResponse.Denied;
        }
        // user clicked 'yes' - validate the data
        else if (model.Button == "yes" && model != null)
        {
            // if the user consented to some scope, build the response model
            if (model.ScopesConsented != null && model.ScopesConsented.Any())
            {
                var scopes = model.ScopesConsented;
                if (ConsentOptions.EnableOfflineAccess == false)
                {
                    scopes = scopes.Where(x => x != "offline_access");
                }

                grantedConsent = new ConsentResponse
                {
                    RememberConsent = model.RememberConsent,
                    ScopesConsented = scopes.ToArray()
                };
            }
            else
            {
                result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
            }
        }
        else
        {
            result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
        }

        if (grantedConsent != null)
        {
            // validate return url is still valid
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null)
            {
                return result;
            }

            // communicate outcome of consent back to identityserver
            await _interaction.GrantConsentAsync(request, grantedConsent);

            // indiate that's it ok to redirect back to authorization endpoint
            result.RedirectUri = model.ReturnUrl;
        }
        else
        {
            // we need to redisplay the consent UI
            result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
        }

        return result;
    }

    public async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
    {
        var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (request != null)
        {
            var client = await _idpm.GetClientAsync(request.ClientId);
            if (client != null && client.Enabled)
            {
                var identityResources = await _idpm.GetEnabledIdentityResourcesByScopeAsync(request.ScopesRequested);
                var apiResources = await _idpm.GetEnabledApiResourcesByScopeAsync(request.ScopesRequested);
                if (identityResources.Any() || apiResources.Any())
                {
                    return CreateConsentViewModel(model, returnUrl, client, identityResources, apiResources);
                }
                else
                {
                    _logger.LogError("No scopes matching: {0}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                }
            }
            else
            {
                _logger.LogError("Invalid client id: {0}", request.ClientId);
            }
        }
        else
        {
            _logger.LogError("No consent request matching request: {0}", returnUrl);
        }

        return null;
    }

    private static ConsentViewModel CreateConsentViewModel(
        ConsentInputModel model, string returnUrl, ClientApiDto client,
        IEnumerable<IdentityResourceDto> identityResources, IEnumerable<ApiResourceApiDto> apiResources)
    {
        var vm = new ConsentViewModel
        {
            RememberConsent = model?.RememberConsent ?? true,
            ScopesConsented = model?.ScopesConsented ?? [],

            ReturnUrl = returnUrl,

            ClientName = client.ClientName,
            ClientUrl = client.ClientUri,
            ClientLogoUrl = client.LogoUri,
            AllowRememberConsent = client.AllowRememberConsent,
        };

        var offlineAccess = apiResources.All(x => x.Scopes.Any(s => s.Name == "offline_access"));

        vm.IdentityScopes = identityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();
        vm.ResourceScopes = [.. apiResources.SelectMany(x => x.Scopes).Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null))];
        if (ConsentOptions.EnableOfflineAccess && offlineAccess)
        {
            vm.ResourceScopes = vm.ResourceScopes.Union([
                GetOfflineAccessScope(vm.ScopesConsented.Contains("offline_access") || model == null)
            ]);
        }

        return vm;
    }

    public static ScopeViewModel CreateScopeViewModel(IdentityResourceDto identity, bool check)
    {
        return new ScopeViewModel
        {
            Name = identity.Name,
            DisplayName = identity.DisplayName,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required,
        };
    }

    public static ScopeViewModel CreateScopeViewModel(ApiScopeApiDto scope, bool check)
    {
        return new ScopeViewModel
        {
            Name = scope.Name,
            DisplayName = scope.DisplayName,
            Description = scope.Description,
            Emphasize = scope.Emphasize,
            Required = scope.Required,
            Checked = check || scope.Required,
        };
    }

    private static ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Name = "offline_access",
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }
}
