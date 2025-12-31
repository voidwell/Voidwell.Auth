using Microsoft.Extensions.DependencyInjection;
using Voidwell.Auth.Admin.Filters;
using Voidwell.Auth.Admin.Services;

namespace Voidwell.Auth.Admin;

public static class AuthAdminExtensions
{
    public static IServiceCollection AddAdminServices(this IServiceCollection services)
    {
        services.AddMvc()
            .AddMvcOptions(options =>
            {
                options.Filters.Add(new SecurityHeadersAttribute());
                options.Filters.Add(new NotFoundExceptionFilter());
                options.Filters.Add(new ConflictExceptionFilter());
            });
        services.AddMvcCore();

        services.AddTransient<IClientService, ClientService>();
        services.AddTransient<IApiResourceService, ApiResourceService>();

        return services;
    }
}
