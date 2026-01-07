using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;

namespace Voidwell.Auth.Data.Repositories;
 
 public interface IIdentityResourceRepository
{
    Task<IEnumerable<IdentityResource>> GetIdentityResourcesAsync();
}