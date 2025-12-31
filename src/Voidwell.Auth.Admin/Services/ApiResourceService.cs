using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Admin.Exceptions;
using Voidwell.Auth.Admin.Mappers;
using Voidwell.Auth.Admin.Models;
using Voidwell.Auth.Data.Models;
using Voidwell.Auth.Data.Repositories;

namespace Voidwell.Auth.Admin.Services;

public class ApiResourceService : IApiResourceService
{
    private readonly IApiResourceRepository _repository;

    public ApiResourceService(IApiResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResourceApiDto> GetApiResourceAsync(string name)
    {
        var id = await GetIdByApiResourceIdAsync(name);
        var apiResource = await _repository.GetApiResourceAsync(id);
        return apiResource.ToModel();
    }

    public async Task<PagedList<ApiResourceApiDto>> GetApiResourcesAsync(string search = "", int page = 1)
    {
        var apiResources = await _repository.GetApiResourcesAsync(search, page);
        return new PagedList<ApiResourceApiDto>(apiResources.Data.Select(c => c.ToModel()), apiResources.PageNumber, apiResources.PageSize, apiResources.TotalCount);
    }

    public async Task<IEnumerable<SecretApiDto>> GetApiResourceSecretsAsync(string name)
    {
        var id = await GetIdByApiResourceIdAsync(name);
        var secrets = await _repository.GetApiResourceSecretsAsync(id);
        return secrets.Select(a => a.ToModel());
    }

    public async Task<ApiResourceApiDto> CreateApiResourceAsync(ApiResourceApiDto apiResource)
    {
        var existing = await _repository.GetIdFromApiResourceName(apiResource.Name);
        if (existing != null)
        {
            throw new ConflictException($"A apiResource with name '{apiResource.Name} already exists.");
        }

        var createdResource = await _repository.AddApiResourceAsync(apiResource.ToEntity());
        return createdResource.ToModel();
    }

    public async Task<ApiResourceApiDto> UpdateApiResourceAsync(string name, ApiResourceApiDto apiResource)
    {
        var id = await GetIdByApiResourceIdAsync(name);

        var scopeNames = apiResource.Scopes.Select(x => x.Name).ToArray();
        var scopeConflicts = await _repository.GetApiResourceScopeConflictsAsync(id, scopeNames);
        if (scopeConflicts.Any())
        {
            throw new ConflictException($"A scope with this name already exists: '{string.Join(", ", scopeConflicts)}'");
        }

        var updatedApiResource = await _repository.UpdateApiResourceAsync(id, apiResource.ToEntity());
        return updatedApiResource.ToModel();
    }

    public async Task RemoveApiResourceAsync(string name)
    {
        var id = await GetIdByApiResourceIdAsync(name);
        await _repository.RemoveApiResourceAsync(id);
    }

    public async Task<CreatedSecretResponse> CreateApiResourceSecretAsync(string name, SecretRequest request)
    {
        var id = await GetIdByApiResourceIdAsync(name);

        var secretValue = Guid.NewGuid().ToString();

        var secret = new ApiSecret
        {
            Created = DateTime.UtcNow,
            Description = request.Description,
            Expiration = request.Expiration,
            Type = "SharedSecret",
            ApiResourceId = id,
            Value = secretValue.Sha256()
        };

        var createdSecret = await _repository.AddApiResourceSecretAsync(id, secret);

        return new CreatedSecretResponse(secretValue, createdSecret.ToModel());
    }

    public async Task DeleteApiResourceSecretAsync(string name, int secretId)
    {
        var id = await GetIdByApiResourceIdAsync(name);

        var secret = await _repository.GetApiResourceSecretAsync(id, secretId);
        if (secret == null)
        {
            return;
        }

        await _repository.DeleteApiResourceSecretAsync(id, secretId);
    }

    private async Task<int> GetIdByApiResourceIdAsync(string name)
    {
        var id = await _repository.GetIdFromApiResourceName(name);
        if (id == null)
        {
            throw new NotFoundException($"ApiResource '{name}' not found");
        }
        return id.Value;
    }
}
