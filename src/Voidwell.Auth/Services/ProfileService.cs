using IdentityServer4.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Extensions;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Voidwell.Auth.Clients;
using System.IO;
using System.Text;

namespace Voidwell.Auth.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManagementClient _userManagementClient;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(IHttpContextAccessor httpContextAccessor, IUserManagementClient userManagementClient, ILogger<ProfileService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManagementClient = userManagementClient;
            _logger = logger;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (Guid.TryParse(context.Subject?.GetSubjectId(), out Guid userId))
            {
                var claims = (await _userManagementClient.GetProfileClaimsAsync(userId)).ToList();

                _logger.LogInformation("Claims: {0}", string.Join(", ", claims.Select(a => $"{a.Type}:{a.Value}")));

                _logger.LogInformation(89415, "Requested claims: {0}", string.Join(", ", context.RequestedClaimTypes));

                context.AddRequestedClaims(claims);
            }
            else
            {
                throw new InvalidOperationException("Could not get subject for profile data");
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;

            if (!Guid.TryParse(context.Subject.GetSubjectId(), out Guid userId))
            {
                _logger.LogInformation("no user");
                return Task.CompletedTask;
            }

            /*
            if (context.Subject.FindFirst(JwtClaimTypes.SessionId)?.Value == null)
            {
                _logger.LogInformation("no session");
                return;
            }
            
            var markActivity = false;
            var requestPath = _httpContextAccessor.HttpContext.Request.Path.ToString();
            if (requestPath == "/connect/introspect" ||
                requestPath == "/connect/token" ||
                requestPath == "/connect/authorize/login")
            {
                markActivity = true;
            }
            */

            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
