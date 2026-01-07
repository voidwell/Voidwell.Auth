using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voidwell.Auth.Data.Repositories;

public class IdentityResourceRepository : IIdentityResourceRepository
{
    private readonly IdentityServerConfigurationDbContext _dbContext;

    public IdentityResourceRepository(IdentityServerConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<IdentityResource>> GetIdentityResourcesAsync()
    {
        return await _dbContext.IdentityResources
            .AsNoTracking()
            .ToListAsync();
    }
}
