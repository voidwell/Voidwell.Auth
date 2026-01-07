using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Voidwell.Auth.Services.Abstractions;
using Voidwell.Auth.UserManagement.Exceptions;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Services;

public class CredentialSignOnService : ICredentialSignOnService
{
    private readonly IUserAuthenticationService _userAuthenticationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;

    public CredentialSignOnService(IUserAuthenticationService userAuthenticationService, IHttpContextAccessor httpContextAccessor, ILogger<CredentialSignOnService> logger)
    {
        _userAuthenticationService = userAuthenticationService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<string> Authenticate(AuthenticationRequest authRequest)
    {
        AuthenticationResult authResult;
        try
        {
            authResult = await _userAuthenticationService.Authenticate(authRequest);
            if (authResult == null)
            {
                return null;
            }

            _logger.LogDebug("Got authentication result for user {UserId}", authResult?.UserId.ToString());
        }
        catch (UserNotFoundException)
        {
            return "User with that username does not exist.";
        }
        catch (InvalidPasswordException)
        {
            return "The password entered is incorrect.";
        }
        catch (UserLockedOutException)
        {
            return "Account has been locked. Please try again later.";
        }

        var name = authResult.Claims.FirstOrDefault(a => a.Type == JwtClaimTypes.Name)?.Value;

        var user = new AuthUser(authResult.UserId.ToString())
        {
            Name = name,
            AuthenticationTime = DateTime.UtcNow,
            AdditionalClaims = [.. authResult.Claims.Where(AdditionalClaimFilter)],
            IdentityProvider = AuthConstants.IdentityProvider
        };
        var principal = user.CreatePrincipal();

        var authProps = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await _httpContextAccessor.HttpContext.SignInAsync(AuthConstants.CookieSchemeName, principal, authProps);
        return null;
    }

    private static bool AdditionalClaimFilter(Claim claim)
    {
        return claim.Type != JwtClaimTypes.Name
            && claim.Type != JwtClaimTypes.Subject;
    }
}
