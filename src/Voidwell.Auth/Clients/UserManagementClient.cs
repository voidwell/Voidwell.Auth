using System;
using System.Net.Http;
using System.Threading.Tasks;
using Voidwell.Auth.HttpAuthenticatedClient;
using Voidwell.Auth.Models;
using System.Collections.Generic;
using System.Security.Claims;
using Voidwell.Auth.Quickstart.Account;

namespace Voidwell.Auth.Clients
{
    public class UserManagementClient : IUserManagementClient, IDisposable
    {
        protected readonly HttpClient _httpClient;

        public UserManagementClient(IAuthenticatedHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.GetHttpClient("voidwell-usermanagement");
            _httpClient.BaseAddress = new Uri("http://voidwellusermanagement:5000");
        }

        public async Task<AuthenticationResult> Authenticate(AuthenticationRequest authRequest)
        {
            var content = JsonContent.FromObject(authRequest);
            var response = await _httpClient.PostAsync("authenticate", content);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.InternalServerError && response.Content != null)
            {
                var error = await response.Content.ReadAsStringAsync();
                return new AuthenticationResult
                {
                    Error = error
                };
            }

            return await response.GetContentAsync<AuthenticationResult>();
        }

        public async Task<IEnumerable<Claim>> GetProfileClaimsAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"profile/{userId}");
            return await response.GetContentAsync<IEnumerable<Claim>>();
        }

        public async Task<IEnumerable<SecurityQuestion>> GetSecurityQuestions(string username)
        {
            var response = await _httpClient.GetAsync($"questions/{username}");
            return await response.GetContentAsync<IEnumerable<SecurityQuestion>>();
        }

        public async Task<IEnumerable<string>> GetRoles(string userId)
        {
            var response = await _httpClient.GetAsync($"roles/{userId}");
            return await response.GetContentAsync<IEnumerable<string>>();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
