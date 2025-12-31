using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Voidwell.Auth.UserManagement.Exceptions;
using Voidwell.Auth.Data.Models;
using System.Linq;
using System.Security.Claims;
using System;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.UserManagement.Services;

public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;
    private readonly ILogger _logger;

    public UserAuthenticationService(UserManager<ApplicationUser> userManager, IUserService userService,
        ILogger<UserAuthenticationService> logger)
    {
        _userManager = userManager;
        _userService = userService;
        _logger = logger;
    }

    public async Task<AuthenticationResult> Authenticate(AuthenticationRequest authRequest)
    {
        var username = authRequest.Username;
        var password = authRequest.Password;

        var user = await _userService.GetUserByEmail(username);

        _logger.LogDebug("Attempting to login user: {User}", username);

        if (user == null)
        {
            _logger.LogWarning("Failed to find user: {User}", username);
            throw new UserNotFoundException();
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("Login attempt for locked out user: {User}", username);
            throw new UserLockedOutException();
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);
            _logger.LogWarning("Failed to authenticate for user: {User}", username);
            throw new InvalidPasswordException();
        }

        _logger.LogDebug("Successfully authenticated user: {User}", username);

        user.LastLoginDate = DateTimeOffset.UtcNow;
        await _userService.UpdateUser(user);

        var claims = (await _userManager.GetClaimsAsync(user)).ToList();

        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        claims.Add(new Claim(ClaimTypes.Name, user.UserName));

        return new AuthenticationResult
        {
            UserId = user.Id,
            Claims = claims
        };
    }
}
