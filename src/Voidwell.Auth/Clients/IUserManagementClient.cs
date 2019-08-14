using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Voidwell.Auth.Models;

namespace Voidwell.Auth.Clients
{
    public interface IUserManagementClient
    {
        Task<AuthenticationResult> Authenticate(AuthenticationRequest authRequest);
        Task<IEnumerable<Claim>> GetProfileClaimsAsync(Guid userId);
        Task<IEnumerable<SecurityQuestion>> GetSecurityQuestions(string username);
        Task<IEnumerable<string>> GetRoles(string userId);
    }
}
