using Microsoft.Extensions.DependencyInjection;
using Voidwell.Auth.UserManagement.Services;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.UserManagement;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserManagementServices(this IServiceCollection services)
    {
        services.AddTransient<IRegistrationService, RegistrationService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<ISecurityQuestionService, SecurityQuestionService>();
        services.AddTransient<IUserAuthenticationService, UserAuthenticationService>();

        services.AddTransient<IUserHelper, UserHelper>();

        return services;
    }
}
