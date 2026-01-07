using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider.Services;

public class ProfileService : IProfileService
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(IUserService userService, ILogger<ProfileService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        if (Guid.TryParse(context.Subject?.GetSubjectId(), out Guid userId))
        {
            var claims = await GetProfileClaimsAsync(userId);

            _logger.LogInformation("Claims: {Claims}", string.Join(", ", claims.Select(a => $"{a.Type}:{a.Value}")));

            _logger.LogInformation(89415, "Requested claims: {Claims}", string.Join(", ", context.RequestedClaimTypes));

            context.AddRequestedClaims(claims);
        }
        else
        {
            throw new InvalidOperationException("Could not get subject for profile data");
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = false;
        if (!Guid.TryParse(context.Subject.GetSubjectId(), out _))
        {
            _logger.LogInformation("no user");
            return;
        }

        if (context.Subject.FindFirst(JwtClaimTypes.SessionId)?.Value == null)
        {
            _logger.LogInformation("no session");
            return;
        }

        context.IsActive = true;
    }

    private async Task<List<Claim>> GetProfileClaimsAsync(Guid userId)
    {
        var user = await _userService.GetUser(userId);
        var roles = await _userService.GetRoles(userId);

        var claims = new List<Claim>
        {
            new(JwtClaimTypes.Subject, userId.ToString()),
            new(JwtClaimTypes.Name, user.UserName),
            new(JwtClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(JwtClaimTypes.Role, role));
        }

        return claims;
    }
}
