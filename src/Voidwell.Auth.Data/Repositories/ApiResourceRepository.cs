using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Voidwell.Auth.Data.Repositories;

public class ApiResourceRepository : IApiResourceRepository
{
    private readonly IdentityServerConfigurationDbContext _dbContext;

    public ApiResourceRepository(IdentityServerConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ApiResource> GetApiResourceAsync(int apiResourceId)
    {
        return _dbContext.ApiResources
            .Include(x => x.UserClaims)
            .Include(x => x.Secrets)
            .Include(x => x.UserClaims)
            .Include(x => x.Properties)
            .Include(x => x.Scopes)
                .ThenInclude(x => x.UserClaims)
            .Where(x => x.Id == apiResourceId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<int?> GetIdFromApiResourceName(string name)
    {
        var apiResource = await _dbContext.ApiResources
            .Where(a => a.Name == name)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        return apiResource?.Id;
    }

    public async Task<PagedList<ApiResource>> GetApiResourcesAsync(string search = "", int page = 1, int pageSize = 10)
    {
        var queryBase = string.IsNullOrEmpty(search) ? _dbContext.ApiResources : _dbContext.ApiResources.Where(a => a.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));

        var apiResourcesTask = queryBase
            .OrderBy(a => a.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var countTask = queryBase.CountAsync();

        await Task.WhenAll(apiResourcesTask, countTask);

        return new PagedList<ApiResource>(apiResourcesTask.Result, page, pageSize, countTask.Result);
    }

    public async Task<ApiSecret> AddApiResourceSecretAsync(int apiResourceId, ApiSecret apiSecret)
    {
        var resource = await _dbContext.ApiResources
            .Where(a => a.Id == apiResourceId)
            .SingleOrDefaultAsync();

        apiSecret.ApiResource = resource;

        await _dbContext.ApiSecrets.AddAsync(apiSecret);

        await _dbContext.SaveChangesAsync();

        return apiSecret;
    }

    public async Task<IEnumerable<ApiSecret>> GetApiResourceSecretsAsync(int apiResourceId)
    {
        return await _dbContext.ApiSecrets
            .Where(x => x.ApiResourceId == apiResourceId)
            .ToListAsync();
    }

    public Task<ApiSecret> GetApiResourceSecretAsync(int apiResourceId, int apiSecretId)
    {
        return _dbContext.ApiSecrets
            .Include(x => x.ApiResource)
            .Where(x => x.ApiResourceId == apiResourceId && x.Id == apiSecretId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task DeleteApiResourceSecretAsync(int apiResourceId, int apiSecretId)
    {
        var secretToDelete = await _dbContext.ApiSecrets.Where(x => x.ApiResourceId == apiResourceId && x.Id == apiSecretId).SingleOrDefaultAsync();

        _dbContext.ApiSecrets.Remove(secretToDelete);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ApiResource> AddApiResourceAsync(ApiResource apiResource)
    {
        _dbContext.ApiResources.Add(apiResource);

        await _dbContext.SaveChangesAsync();

        return apiResource;
    }

    public async Task<ApiResource> UpdateApiResourceAsync(int apiResourceId, ApiResource apiResource)
    {
        await RemoveApiResourceRelationsAsync(apiResourceId);

        _dbContext.ApiResources.Update(apiResource);

        await _dbContext.SaveChangesAsync();

        return apiResource;
    }

    public async Task RemoveApiResourceAsync(int apiResourceId)
    {
        var apiResource = await _dbContext.ApiResources.Where(a => a.Id == apiResourceId).FirstOrDefaultAsync();

        _dbContext.ApiResources.Remove(apiResource);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetApiResourceScopeConflictsAsync(int apiResourceId, params string[] scopeNames)
    {
        var conflictScopes = await _dbContext.ApiScopes
            .Where(x => scopeNames.Contains(x.Name) && x.ApiResourceId != apiResourceId)
            .AsNoTracking()
            .ToListAsync();

        return conflictScopes.Select(a => a.Name);
    }

    private async Task RemoveApiResourceRelationsAsync(int apiResourceId)
    {
        var ApiResourceScopesTask = _dbContext.ApiScopes.Where(x => x.ApiResourceId == apiResourceId).ToListAsync();
        var apiResourceClaimsTask = _dbContext.ApiResourceClaims.Where(x => x.ApiResourceId == apiResourceId).ToListAsync();
        var apiResourcePropertiesTask = _dbContext.ApiResourceProperties.Where(x => x.ApiResourceId == apiResourceId).ToListAsync();

        await Task.WhenAll(
            ApiResourceScopesTask,
            apiResourceClaimsTask,
            apiResourcePropertiesTask);

        _dbContext.ApiScopes.RemoveRange(ApiResourceScopesTask.Result);
        _dbContext.ApiResourceClaims.RemoveRange(apiResourceClaimsTask.Result);
        _dbContext.ApiResourceProperties.RemoveRange(apiResourcePropertiesTask.Result);
    }
}
