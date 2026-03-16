using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Voidwell.Auth.Data;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.UserManagement;
using Voidwell.Auth.UserManagement.Services.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Voidwell.Auth.Seeding;

internal class SeedingService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SeedingConfig _config;

    public SeedingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _config = _serviceProvider.GetService<IOptions<SeedingConfig>>()?.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var serviceScope = _serviceProvider.CreateAsyncScope();

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (_config == null || !_config.TrySeeding)
        {
            return;
        }

        await CreateAdminUiScopeAsync();
        await CreateInitialUserRolesAsync();

        await CreateAdminApiClientAsync();
        await CreateInitialAdminUserAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task CreateAdminUiScopeAsync()
    {
        var scopeManager = _serviceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync("voidwell-auth-admin") != null)
        {
            return;
        }

        await scopeManager.CreateAsync(new AuthScopeDescriptor
        {
            Name = "voidwell-auth-admin",
            Description = "Access to Voidwell Auth Admin"
        });
    }
    
    private async Task CreateAdminApiClientAsync()
    {
        var applicationManager = _serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await applicationManager.FindByClientIdAsync("admin-api") != null)
        {
            return;
        }

        await applicationManager.CreateAsync(new AuthApplicationDescriptor
        {
            ClientId = "admin-api",
            Enabled = true,
            ClientType = ClientTypes.Confidential,
            ConsentType = ConsentTypes.Implicit,
            DisplayName = "Admin API Client",
            Permissions =
                {
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.Endpoints.Token,
                    Permissions.Prefixes.Scope + "voidwell-auth-admin"
                },
            ClientSecret = _config.AdminApiSecret
        });
    }

    private async Task CreateInitialUserRolesAsync()
    {
        string[] seedRoles = [
            UserRole.SuperAdmin.ToString(),
            UserRole.Administrator.ToString(),
            UserRole.EventManager.ToString(),
            UserRole.User.ToString()
        ];

        var roleService = _serviceProvider.GetRequiredService<IRoleService>();

        var existingRoles = await roleService.GetAllRolesAsync();
        foreach (var role in seedRoles)
        {
            if (existingRoles.Any(a => a.Name == role))
            {
                continue;
            }

            await roleService.CreateRoleAsync(role);
        }
    }

    private async Task CreateInitialAdminUserAsync()
    {
        var userService = _serviceProvider.GetRequiredService<IUserService>();

        var superAdmins = await userService.GetUsersByRoleAsync(UserRole.SuperAdmin.ToString());
        if (superAdmins != null && superAdmins.Any())
        {
            return;
        }

        if (await userService.GetUserByEmail(_config.AdminUserEmail) != null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_config.AdminUserEmail) || string.IsNullOrWhiteSpace(_config.AdminUserPassword))
        {
            throw new InvalidOperationException("Admin user email and password must be provided in configuration when seeding if the user doesn't already exist.");
        }

        var adminUser = await userService.CreateUser(_config.AdminUserEmail, _config.AdminUserEmail, _config.AdminUserPassword);
        await userService.AddRoleAsync(adminUser.Id, UserRole.SuperAdmin.ToString());
    }
}
