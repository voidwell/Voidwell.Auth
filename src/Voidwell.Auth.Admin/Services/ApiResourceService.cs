using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.IdentityServer.Models;
using Voidwell.Auth.IdentityServer.Services.Abstractions;

namespace Voidwell.Auth.Admin.Services;

public class ApiResourceService : IApiResourceService
{
    private const int _pageSize = 50;

    private readonly IIdentityProviderManager _idpm;

    public ApiResourceService(IIdentityProviderManager idpm)
    {
        _idpm = idpm;
    }

    public async Task<ApiResourceApiDto> GetApiResourceAsync(string name)
    {
        return await _idpm.GetApiResourceAsync(name);
    }

    public async Task<PagedList<ApiResourceApiDto>> GetApiResourcesAsync(string search = "", int page = 1)
    {
        var results = await _idpm.GetApiResourcesAsync();

        if (!string.IsNullOrEmpty(search))
        {
            results = results.Where(c => c.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }

        var pageList = results.Take(_pageSize).Skip(_pageSize * page);

        return new PagedList<ApiResourceApiDto>(pageList, page, _pageSize, results.Count());
    }

    public async Task<IEnumerable<SecretApiDto>> GetApiResourceSecretsAsync(string name)
    {
        return await _idpm.GetApiResourceSecretsAsync(name);
    }

    public async Task<ApiResourceApiDto> CreateApiResourceAsync(ApiResourceApiDto apiResource)
    {
        return await _idpm.CreateApiResourceAsync(apiResource);
    }

    public async Task<ApiResourceApiDto> UpdateApiResourceAsync(string name, ApiResourceApiDto apiResource)
    {
        return await _idpm.UpdateApiResourceAsync(name, apiResource);
    }

    public async Task RemoveApiResourceAsync(string name)
    {
        await _idpm.DeleteApiResourceAsync(name);
    }

    public async Task<CreatedSecretResponse> CreateApiResourceSecretAsync(string name, SecretRequest request)
    {
        var secretDto = new SecretApiDto
        {
            Description = request.Description,
            Expiration = request.Expiration,
            Type = "SharedSecret",
            Value = Guid.NewGuid().ToString(),
            Created = DateTime.UtcNow
        };

        var createdSecret = await _idpm.AddApiResourceSecretAsync(name, secretDto);

        return new CreatedSecretResponse(secretDto.Value, createdSecret);
    }

    public async Task DeleteApiResourceSecretAsync(string name, int secretId)
    {
        await _idpm.DeleteApiResourceSecretAsync(name, secretId);
    }
}
