using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using IdentityModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Voidwell.Auth;
using Voidwell.Auth.Admin;
using Voidwell.Auth.Data;
using Voidwell.Auth.Delegation;
using Voidwell.Auth.Services;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    RequireHeaderSymmetry = false,
    ForwardLimit = 15,
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeaderOptions.KnownIPNetworks.Clear();
forwardedHeaderOptions.KnownProxies.Clear();

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables();

// Logging
builder.Logging
    .ClearProviders()
    .AddServiceLogging(builder.Environment, builder.Configuration);

// Services
builder.Services.AddMvcCore()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .AddDataAnnotations()
    .AddAuthorization(options =>
    {
        options.AddPolicy("IsAdmin",
            policy => policy.AddAuthenticationSchemes("Bearer")
                            .RequireClaim(JwtClaimTypes.Scope, "voidwell-auth-admin"));
    });

builder.Services.AddAuthentication("voidwell")
        .AddCookie("voidwell", options =>
        {
            options.SlidingExpiration = false;
            options.ExpireTimeSpan = TimeSpan.FromHours(10);
            options.Cookie = new CookieBuilder
            {
                Name = "voidwell",
                SecurePolicy = CookieSecurePolicy.SameAsRequest
            };
        })
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "https://auth.voidwell.com";
            options.Audience = "voidwell-auth";
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
        });

builder.Services.AddIdentityServer(options =>
    {
        options.IssuerUri = builder.Configuration.GetValue<string>("Issuer");

        options.Discovery.ShowIdentityScopes = false;
        options.Discovery.ShowApiScopes = false;
        options.Discovery.ResponseCacheInterval = 60 * 60;

        options.InputLengthRestrictions.Scope = 800;

        options.Events.RaiseSuccessEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseErrorEvents = false;
    })
    .AddDeveloperSigningCredential()
    .AddIdentityServerStores(builder.Configuration)
    .AddAspNetIdentityStores(builder.Configuration)
    .AddProfileService<ProfileService>()
    .AddExtensionGrantValidator<DelegationGrantValidator>();

builder.Services
    .AddAntiforgery(options =>
    {
        options.Cookie = new CookieBuilder
        {
            Name = "voidwell.xsrf",
            SecurePolicy = CookieSecurePolicy.SameAsRequest
        };
    })
    .AddCors()

    .AddEntityFrameworkContext(builder.Configuration)
    .AddAdminServices()
    .AddUserManagementServices()

    .AddSingleton<IClaimsTransformation, ClaimsTransformer>()
    .AddTransient<Func<ITokenCreationService>>(a => () => a.GetService<ITokenCreationService>())
    .AddTransient<IDelegationTokenValidationService, DelegationTokenValidationService>()
    .AddTransient<IDelegationGrantValidationService, DelegationGrantValidationService>()
    .AddTransient<ICorsPolicyService, CorsPolicyService>()
    .AddTransient<IProfileService, ProfileService>()
    .AddTransient<ICredentialSignOnService, CredentialSignOnService>()
    .AddTransient<IAccountService, AccountService>()
    .AddTransient<IConsentHandler, ConsentHandler>();

// Build
var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app
    .InitializeDatabases(app.Configuration)
    .UseForwardedHeaders(forwardedHeaderOptions)
    .UseAuthentication()
    .UseStaticFiles()
    .UseIdentityServer()
    .UseMvc();

await app.RunAsync();
