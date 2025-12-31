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
using Voidwell.Auth.IdentityServer;
using Voidwell.Auth.IdentityServer.Services;
using Voidwell.Auth.Services;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement;
using IConsentService = Voidwell.Auth.Services.Abstractions.IConsentService;

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
    .AddTokenServer(builder.Configuration)
    .AddAdminServices()
    .AddUserManagementServices()

    .AddSingleton<IClaimsTransformation, ClaimsTransformer>()
    .AddTransient<ICredentialSignOnService, CredentialSignOnService>()
    .AddTransient<IAccountService, AccountService>()
    .AddScoped<IConsentService, ConsentService>();

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
