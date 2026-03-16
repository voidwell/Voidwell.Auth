using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.Auth.Data;
using Voidwell.Auth.Data.Entities;
using Voidwell.Auth.UserManagement.Services;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.UserManagement;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserManagement(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(identity =>
        {
            identity.User.RequireUniqueEmail = true;
            identity.Password.RequireDigit = false;
            identity.Password.RequireNonAlphanumeric = false;
            identity.Password.RequireLowercase = false;
            identity.Password.RequireUppercase = false;
            identity.Password.RequiredLength = 6;

            identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            identity.Lockout.MaxFailedAccessAttempts = 5;
        })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ISecurityQuestionService, SecurityQuestionService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();

        return services;
    }
}
