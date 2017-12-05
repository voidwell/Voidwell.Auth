using System.Net.Http;

namespace Voidwell.Auth.HttpAuthenticatedClient
{
    public interface IAuthenticatedHttpClientFactory
    {
        HttpClient GetHttpClient(string targetScope);
    }
}