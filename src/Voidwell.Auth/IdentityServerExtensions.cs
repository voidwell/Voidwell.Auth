using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Voidwell.Auth.Data;
using Microsoft.Extensions.Configuration;

namespace Voidwell.Auth
{
    public static class IdentityServerExtensions
    {
        public static IServiceCollection AddVoidwellIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkContext(configuration);

            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddIdentityServerStores(configuration);
            //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            return services;
        }

        public static IApplicationBuilder UseVoidwellIdentityServer(this IApplicationBuilder app)
        {
            app.UseIdentityServer();

            return app;
        }
    }
}
