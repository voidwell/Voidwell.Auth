using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Voidwell.Auth.Admin.Services;
using Voidwell.Auth.Admin.Components;

namespace Voidwell.Auth.Admin;

public static class AuthAdminExtensions
{
    public static IServiceCollection AddAdminServices(this IServiceCollection services)
    {
        // Add Blazor services
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Add MudBlazor services
        services.AddMudServices();

        services.AddScoped<IClientService, ClientService>();

        return services;
    }

    public static WebApplication UseAdminApp(this WebApplication app)
    {
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .RequireAuthorization("IsAdminUser");
        app.MapStaticAssets();

        return app;
    }
}
