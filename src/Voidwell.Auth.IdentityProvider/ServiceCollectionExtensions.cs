using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Voidwell.Auth.Data;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.IdentityProvider.GrantValidators;
using Voidwell.Auth.IdentityProvider.Handlers;
using Voidwell.Auth.IdentityProvider.Services;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTokenServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<AuthDbContext>()
                       .ReplaceDefaultEntities<AuthApplication, AuthAuthorization, AuthScope, AuthToken, int>();
            })
            .AddServer(options =>
            {
                options.SetIssuer(new Uri(configuration.GetValue<string>("Issuer")));

                // Enable the endpoints
                options.SetTokenEndpointUris("connect/token");
                options.SetAuthorizationEndpointUris("connect/authorize");
                options.SetUserInfoEndpointUris("connect/userinfo");
                options.SetEndSessionEndpointUris("connect/endsession");
                options.SetRevocationEndpointUris("connect/revocation");
                options.SetIntrospectionEndpointUris("connect/introspect");

                // Enable the flows
                options.AllowPasswordFlow();
                options.AllowRefreshTokenFlow();
                options.AllowAuthorizationCodeFlow()
                       .RequireProofKeyForCodeExchange();
                options.AllowClientCredentialsFlow();
                options.AllowImplicitFlow();
                options.AllowCustomFlow("delegation");

                // Configure encryption and signing credentials
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Use self-contained JWT tokens by default (APIs can validate without introspection)
                // Disable encryption so APIs can validate tokens directly
                options.DisableAccessTokenEncryption();

                // Register ASP.NET Core host
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableUserInfoEndpointPassthrough()
                    .EnableEndSessionEndpointPassthrough();

                // Add custom handler for per-client reference token support
                // This converts JWT tokens to reference tokens for clients configured with AccessTokenType = "reference"
                options.AddEventHandler(AccessTokenFormatHandler.Descriptor);
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        services
            .AddScoped<IIdentityProviderManager, IdentityProviderManager>()
            .AddScoped<ISecretManager, SecretManager>()
            .AddScoped<IOpenIddictApplicationManager, AuthApplicationManager>()
            .AddScoped<AccessTokenFormatHandler>()
            .AddScoped<RequireAccessTokenGenerated>();

        // Register grant validator infrastructure
        services
            .AddScoped<IGrantValidatorProvider, GrantValidatorProvider>();
        // Register all grant validators
        services
            .AddScoped<IGrantValidator, AuthorizationCodeGrantValidator>()
            .AddScoped<IGrantValidator, RefreshTokenGrantValidator>()
            .AddScoped<IGrantValidator, PasswordGrantValidator>()
            .AddScoped<IGrantValidator, ClientCredentialsGrantValidator>()
            .AddScoped<IGrantValidator, DeviceCodeGrantValidator>()
            .AddScoped<IGrantValidator, DelegationGrantValidator>();

        return services;
    }
}
