using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.Models;
using IConsentService = Voidwell.Auth.Services.Abstractions.IConsentService;

namespace Voidwell.Auth.Services;

public class ConsentService : IConsentService
{
    private static readonly AuthScope _offlineAccessScope = new()
    {
        Name = "offline_access",
        DisplayName = ConsentOptions.OfflineAccessDisplayName,
        Description = ConsentOptions.OfflineAccessDescription
    };

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly ILogger<ConsentService> _logger;

    public ConsentService(IHttpContextAccessor httpContextAccessor, IOpenIddictApplicationManager applicationManager, IOpenIddictScopeManager scopeManager, ILogger<ConsentService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
        _logger = logger;
    }

    public IEnumerable<string> GetConsentedScopes(ConsentInputModel model)
    {
        var request = _httpContextAccessor.HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var requestedScopes = request.GetScopes();

        var scopes = model.ScopesConsented.Intersect(requestedScopes);

        if (ConsentOptions.EnableOfflineAccess == false)
        {
            scopes = scopes.Where(x => x != _offlineAccessScope.Name);
        }

        return scopes.ToArray();
    }

    public async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
    {
        var request = _httpContextAccessor.HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var client = (AuthApplication) await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
            throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

        if (!client.Enabled)
        {
            throw new InvalidOperationException("Application is disabled.");
        }

        var requestedScopes = request.GetScopes();
        var resources = await _scopeManager.ListResourcesAsync(requestedScopes).ToListAsync();

        var resourceScopes = new List<AuthScope>();
        foreach (var resource in requestedScopes)
        {
            resourceScopes.AddRange(await _scopeManager.FindByResourceAsync(resource).Cast<AuthScope>().ToListAsync());
        }

        if (!resourceScopes.Any())
        {
            _logger.LogError("No scopes matching: {0}", requestedScopes.Aggregate((x, y) => x + ", " + y));
            return null;
        }

        return CreateConsentViewModel(model, request.RedirectUri, client, resourceScopes);
    }

    private static ConsentViewModel CreateConsentViewModel(ConsentInputModel model, string returnUrl, AuthApplication client, IEnumerable<AuthScope> resourceScopes)
    {
        var vm = new ConsentViewModel
        {
            RememberConsent = model?.RememberConsent ?? true,
            ScopesConsented = model?.ScopesConsented ?? System.Array.Empty<string>(),

            ReturnUrl = returnUrl,

            ClientName = client.DisplayName,
            ClientUrl = client.ClientUri,
            ClientLogoUrl = client.ClientLogoUri,
            AllowRememberConsent = client.AllowRememberConsent,
        };

        vm.Scopes = resourceScopes.Select(scope => CreateScopeViewModel(scope, vm.ScopesConsented.Contains(scope.Name) || model == null)).ToList();
        if (ConsentOptions.EnableOfflineAccess && resourceScopes.Any(s => s.Name == _offlineAccessScope.Name))
        {
            vm.Scopes = [.. vm.Scopes.Union([CreateScopeViewModel(_offlineAccessScope, vm.ScopesConsented.Contains(_offlineAccessScope.Name) || model == null)])];
        }

        return vm;
    }

    public static ScopeViewModel CreateScopeViewModel(AuthScope scope, bool check)
    {
        return new ScopeViewModel
        {
            Name = scope.Name,
            DisplayName = scope.DisplayName,
            Description = scope.Description,
            Checked = check
        };
    }
}
