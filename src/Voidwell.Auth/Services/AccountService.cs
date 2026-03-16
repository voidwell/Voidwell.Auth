using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;
using Voidwell.Auth.Models;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.Services;

public class AccountService : IAccountService
{
    private readonly IIdentityProviderManager _idpm;
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountService(
        IHttpContextAccessor httpContextAccessor,
        IIdentityProviderManager idpm,
        IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _idpm = idpm;
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
    {
        var request = _httpContextAccessor.HttpContext.GetOpenIddictServerRequest();

        if (request?.IdentityProvider != null)
        {
            // this is meant to short circuit the UI and only trigger the one external IdP
            return new LoginViewModel
            {
                EnableLocalLogin = false,
                ReturnUrl = returnUrl,
                Username = request?.LoginHint,
                ExternalProviders = new ExternalProvider[]
                {
                    new ExternalProvider { AuthenticationScheme = request.IdentityProvider }
                }
            };
        }

        var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null && !AccountOptions.WindowsAuthenticationSchemes.Contains(x.Name))
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName,
                AuthenticationScheme = x.Name
            }).ToList();

        var allowLocal = true;
        if (request?.ClientId != null)
        {
            var client = await _idpm.GetClientAsync(request.ClientId);
            if (client != null && client.Enabled)
            {
                allowLocal = client.EnableLocalLogin;
            }
        }

        var model = new LoginViewModel
        {
            AllowRememberLogin = AccountOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
            ReturnUrl = returnUrl,
            Username = request?.LoginHint,
            ExternalProviders = [.. providers]
        };

        return model;
    }

    public async Task<LoginViewModel> BuildLoginViewModelAsync(AuthenticationRequest authRequest)
    {
        var vm = await BuildLoginViewModelAsync(authRequest.ReturnUrl);
        vm.Username = authRequest.Username;
        vm.RememberLogin = authRequest.RememberLogin;
        return vm;
    }
}
