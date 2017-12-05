using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Services;

namespace Voidwell.Auth.HttpAuthenticatedClient
{
    public class AuthenticatedHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<ITokenCreationService> _tokenCreationServiceFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly HttpMessageInvoker _httpMessageInvoker;
        private readonly IdentityServerOptions _options;

        public const string ClientId = "voidwell-auth";
        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromSeconds(60);
        public string Scope { get; set; }

        public AuthenticatedHttpMessageHandler(Func<ITokenCreationService> tokenCreationServiceFactory,
            IHttpContextAccessor httpContextAccessor, IdentityServerOptions options,
            ILogger<AuthenticatedHttpMessageHandler> logger)
        {
            _tokenCreationServiceFactory = tokenCreationServiceFactory;
            _httpContextAccessor = httpContextAccessor;
            _options = options;
            _logger = logger;

            var handler = new HttpClientHandler();
            _httpMessageInvoker = new HttpMessageInvoker(handler, true);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Scope))
            {
                throw new InvalidOperationException($"{nameof(Scope)} must be set before sending a request.");
            }

            var token = await GetClientTokenAsync(Scope);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
                _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None);

            return await _httpMessageInvoker.SendAsync(request, linkedCancellationTokenSource.Token);
        }

        private Task<string> GetClientTokenAsync(string scope)
        {
            if (string.IsNullOrEmpty(scope))
            {
                throw new ArgumentException("Scope must be set", nameof(scope));
            }

            var token = new Token
            {
                Issuer = _options.IssuerUri,
                Audiences = new List<string> { $"{_options.IssuerUri}/resources" },
                Lifetime = (int)TokenLifetime.TotalSeconds,
                Claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.ClientId, ClientId),
                    new Claim(JwtClaimTypes.Scope, scope)
                }
            };

            return _tokenCreationServiceFactory().CreateTokenAsync(token);
        }

        protected override void Dispose(bool disposing)
        {
            _httpMessageInvoker.Dispose();
            base.Dispose(disposing);
        }
    }
}
