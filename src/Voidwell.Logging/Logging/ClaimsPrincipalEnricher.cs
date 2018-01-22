using IdentityModel;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.Common.Logging
{
    public class ClaimsPrincipalEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsPrincipalEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_httpContextAccessor?.HttpContext?.User == null)
            {
                return;
            }

            GetProperties()
                .Select(a => propertyFactory.CreateProperty(a.Key, a.Value))
                .ToList()
                .ForEach(logEvent.AddPropertyIfAbsent);
        }

        public IEnumerable<KeyValuePair<string, string>> GetProperties()
        {
            var user = _httpContextAccessor.HttpContext.User;

            var userClaim = user.FindFirst(JwtClaimTypes.Subject);
            var clientClaim = user.FindFirst(JwtClaimTypes.ClientId);
            var sessionClaim = user.FindFirst(JwtClaimTypes.SessionId);

            if (userClaim != null)
            {
                yield return new KeyValuePair<string, string>("User", userClaim.Value);
            }

            if (clientClaim != null)
            {
                yield return new KeyValuePair<string, string>("Client", clientClaim.Value);
            }

            if (sessionClaim != null)
            {
                yield return new KeyValuePair<string, string>("Session", sessionClaim.Value);
            }
        }
    }
}
