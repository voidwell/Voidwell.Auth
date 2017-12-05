using System;
using System.Net.Http;

namespace Voidwell.Auth.HttpAuthenticatedClient
{
    public class AuthenticatedHttpClientFactory : IAuthenticatedHttpClientFactory
    {
        private readonly Func<string, AuthenticatedHttpMessageHandler> _authenticatedHttpMessageHandlerFactory;

        public AuthenticatedHttpClientFactory(Func<string, AuthenticatedHttpMessageHandler> authenticatedHttpMessageHandlerFactory)
        {
            _authenticatedHttpMessageHandlerFactory = authenticatedHttpMessageHandlerFactory;
        }

        public HttpClient GetHttpClient(string targetScope)
        {
            return new HttpClient(_authenticatedHttpMessageHandlerFactory(targetScope), true);
        }
    }
}
