using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Voidwell.Auth.HttpAuthenticatedClient;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticatedHttpClientExtensions
    {
        public static void AddAuthenticatedHttpClient(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IAuthenticatedHttpClientFactory, AuthenticatedHttpClientFactory>();
            services.AddTransient<AuthenticatedHttpMessageHandler>();
            services.AddTransient<Func<string, AuthenticatedHttpMessageHandler>>(
                sp => scope =>
                {
                    var handler = sp.GetService<AuthenticatedHttpMessageHandler>();
                    handler.Scope = scope;
                    return handler;
                });
        }
    }
}
