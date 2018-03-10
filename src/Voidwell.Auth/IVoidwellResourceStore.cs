using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.Auth
{
    public interface IVoidwellResourceStore: IResourceStore
    {
        Task<IEnumerable<ApiResource>> GetAllApiResourcesAsync();
        Task<ApiResource> CreateApiResourceAsync(ApiResource apiResource);
        Task<ApiResource> UpdateApiResourceAsync(string apiResourceId, ApiResource apiResource);
        Task<Guid> CreateSecretAsync(string apiResourceId, string description, DateTime? expiration = null);
        Task DeleteSecretAsync(string apiResourceId, string description);
    }
}