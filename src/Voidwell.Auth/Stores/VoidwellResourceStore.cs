using System;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Voidwell.Auth.Stores
{
    public class VoidwellResourceStore : ResourceStore, IVoidwellResourceStore
    {
        private readonly IConfigurationDbContext _context;

        public VoidwellResourceStore(IConfigurationDbContext context, ILogger<VoidwellResourceStore> logger) : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ApiResource>> GetAllApiResourcesAsync()
        {
            var apiResources = await _context.ApiResources.ToListAsync();
            return apiResources.Select(a => a.ToModel()).AsEnumerable();
        }

        public async Task<ApiResource> CreateApiResourceAsync(ApiResource newResource)
        {
            var existing = await FindApiResourceAsync(newResource.Name);
            if (existing != null)
            {
                throw new Exception();
            }

            newResource.ApiSecrets.Clear();
            var entity = newResource.ToEntity();

            _context.ApiResources.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToModel();
        }

        public async Task<ApiResource> UpdateApiResourceAsync(string apiResourceId, ApiResource updatedResource)
        {
            var existing = await FindApiResourceAsync(apiResourceId);
            if (existing == null)
            {
                throw new Exception();
            }

            updatedResource.ApiSecrets = existing.ApiSecrets;
            var entity = updatedResource.ToEntity();

            _context.ApiResources.Update(entity);
            await _context.SaveChangesAsync();

            var savedModel = entity.ToModel();
            savedModel.ApiSecrets.Clear();

            return savedModel;
        }

        public async Task<Guid> CreateSecretAsync(string apiResourceId, string description, DateTime? expiration = null)
        {
            var apiResource = await FindApiResourceAsync(apiResourceId);
            if (apiResource == null)
            {
                throw new Exception();
            }

            if (apiResource.ApiSecrets.Any(a => a.Description == description))
            {
                throw new Exception();
            }

            var secretValue = Guid.NewGuid();
            var secret = new Secret(secretValue.ToString(), description, expiration);

            apiResource.ApiSecrets.Add(secret);

            var entity = apiResource.ToEntity();
            _context.ApiResources.Update(entity);
            await _context.SaveChangesAsync();

            return secretValue;
        }

        public async Task DeleteSecretAsync(string apiResourceId, string description)
        {
            var apiResource = await FindApiResourceAsync(apiResourceId);
            if (apiResource == null)
            {
                throw new Exception();
            }

            var secret = apiResource.ApiSecrets.FirstOrDefault(a => a.Description == description);
            if (secret == default(Secret))
            {
                throw new Exception();
            }

            apiResource.ApiSecrets.Remove(secret);

            var entity = apiResource.ToEntity();
            _context.ApiResources.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
