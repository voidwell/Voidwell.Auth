using System.Threading.Tasks;
using Voidwell.Auth.UserManagement.Models;

namespace Voidwell.Auth.Services.Abstractions;

public interface ICredentialSignOnService
{
    Task<string> AuthenticateAsync(AuthenticationRequest authRequest);
}