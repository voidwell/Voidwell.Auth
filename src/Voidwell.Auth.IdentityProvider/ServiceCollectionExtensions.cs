using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.Auth.Data;
using Voidwell.Auth.IdentityProvider.Delegation;
using Voidwell.Auth.IdentityProvider.Services;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTokenServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityServer(options =>
        {
            options.IssuerUri = configuration.GetValue<string>("Issuer");

            options.Discovery.ShowIdentityScopes = false;
            options.Discovery.ShowApiScopes = false;
            options.Discovery.ResponseCacheInterval = 60 * 60;

            options.InputLengthRestrictions.Scope = 800;

            options.Events.RaiseSuccessEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseErrorEvents = false;
        })
            .AddDeveloperSigningCredential()
            .AddIdentityServerStores(configuration)
            .AddAspNetIdentityStores(configuration)
            .AddProfileService<ProfileService>()
            .AddExtensionGrantValidator<DelegationGrantValidator>()
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

        services
            .AddScoped<IIdentityProviderManager, IdentityProviderManager>()
            .AddScoped<IIdentityProviderInteractionService, IdentityProviderInteractionService>()
            .AddTransient<ICorsPolicyService, AuthCorsPolicyService>()
            .AddTransient<IDelegationTokenValidationService, DelegationTokenValidationService>()
            .AddTransient<IDelegationGrantValidationService, DelegationGrantValidationService>();

        return services;
    }
}
