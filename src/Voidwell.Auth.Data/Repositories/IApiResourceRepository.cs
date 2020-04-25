using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Auth.Data.Models;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Voidwell.Auth.Data.Repositories
{
    public interface IApiResourceRepository
    {
        Task<ApiResource> GetApiResourceAsync(int apiResourceId);
        Task<int?> GetIdFromApiResourceName(string name);
        Task<PagedList<ApiResource>> GetApiResourcesAsync(string search = "", int page = 1, int pageSize = 10);
        Task<ApiSecret> AddApiResourceSecretAsync(int apiResourceId, ApiSecret apiSecret);
        Task<IEnumerable<ApiSecret>> GetApiResourceSecretsAsync(int apiResourceId);
        Task<ApiSecret> GetApiResourceSecretAsync(int apiResourceId, int apiSecretId);
        Task DeleteApiResourceSecretAsync(int apiResourceId, int apiSecretId);
        Task<ApiResource> AddApiResourceAsync(ApiResource apiResource);
        Task<ApiResource> UpdateApiResourceAsync(int apiResourceId, ApiResource apiResource);
        Task RemoveApiResourceAsync(int apiResourceId);
    }
}