using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Voidwell.VoidwellAuth.Data;
using Voidwell.VoidwellAuth.IdentityServer.Services;

namespace Voidwell.VoidwellAuth.IdentityServer
{
    public static class IdentityServerExtensions
    {
        public static IServiceCollection AddVoidwellIdentityServer(this IServiceCollection services)
        {
            services.AddEntityFrameworkContext();

            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddIdentityServerStores();
            //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IUserCryptography, UserCryptography>();

            return services;
        }

        public static IApplicationBuilder UserVoidwellIdentityServer(this IApplicationBuilder app)
        {
            app.UseIdentityServer();

            return app;
        }
    }
}
