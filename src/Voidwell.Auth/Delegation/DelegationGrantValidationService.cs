using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using IdentityServer4.Models;
using IdentityModel;

namespace Voidwell.Auth.Delegation
{
    public class DelegationGrantValidationService : IDelegationGrantValidationService
    {
        private readonly IDelegationTokenValidationService _delegationTokenValidationService;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, AsyncLazy<TokenValidationResult>> _lazyTokenValidationResults;

        public DelegationGrantValidationService(IDelegationTokenValidationService delegationTokenValidationService,
            ILogger<DelegationGrantValidationService> logger)
        {
            _delegationTokenValidationService = delegationTokenValidationService;
            _logger = logger;
            _lazyTokenValidationResults = new ConcurrentDictionary<string, AsyncLazy<TokenValidationResult>>();
        }

        public async Task<GrantValidationResult> GetGrantValidationResultAsync(string userToken)
        {
            var lazyGrantValidation = _lazyTokenValidationResults.GetOrAdd(userToken, CreateLazyTokenValidationResult);
            TokenValidationResult tokenValidationResult = null;

            try
            {
                tokenValidationResult = await lazyGrantValidation.Value;
            }
            finally
            {
                // Always remove response from _lazyGrantValidationResults when complete
                AsyncLazy<TokenValidationResult> removed;
                _lazyTokenValidationResults.TryRemove(userToken, out removed);
            }

            if (tokenValidationResult.IsError)
            {
                _logger.LogWarning("{0} Error when validating access type for grant delegation: {1}",
                    tokenValidationResult.ErrorDescription, tokenValidationResult.Error);

                return new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }

            var subjectClaim = tokenValidationResult.Claims?.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);

            if (subjectClaim == null)
            {
                var formattedClaims = tokenValidationResult.Claims.Select(a => $"{a.Type}: {a.Value}");
                var claims = string.Join(Environment.NewLine, formattedClaims);

                _logger.LogWarning("Delegation failed because token did not include a subject. Other claims were \r\n{0}", claims);

                return new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }

            var identityProvider = tokenValidationResult.Claims?
                .FirstOrDefault(c => c.Type == JwtClaimTypes.IdentityProvider)?.Value;

            var grantValidationResult = new GrantValidationResult(subjectClaim.Value, "delegation", identityProvider: identityProvider ?? "unknown");

            _logger.LogInformation("Delegation GrantValidationResult complete");

            return grantValidationResult;
        }

        private AsyncLazy<TokenValidationResult> CreateLazyTokenValidationResult(string token)
        {
            return new AsyncLazy<TokenValidationResult>(() =>
                _delegationTokenValidationService.GetTokenValidationResultAsync(token));
        }
    }
}
