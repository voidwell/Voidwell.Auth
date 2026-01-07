using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.IdentityServer.Models;

namespace Voidwell.Auth.Admin.Services;

public interface IApiResourceService
{
    Task<ApiResourceApiDto> GetApiResourceAsync(string name);
    Task<PagedList<ApiResourceApiDto>> GetApiResourcesAsync(string search = "", int page = 1);
    Task<IEnumerable<SecretApiDto>> GetApiResourceSecretsAsync(string name);
    Task<ApiResourceApiDto> CreateApiResourceAsync(ApiResourceApiDto apiResource);
    Task<ApiResourceApiDto> UpdateApiResourceAsync(string name, ApiResourceApiDto apiResource);
    Task RemoveApiResourceAsync(string name);
    Task<CreatedSecretResponse> CreateApiResourceSecretAsync(string name, SecretRequest request);
    Task DeleteApiResourceSecretAsync(string name, int secretId);
}