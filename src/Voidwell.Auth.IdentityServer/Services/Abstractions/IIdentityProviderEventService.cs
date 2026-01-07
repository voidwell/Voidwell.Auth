using System.Threading.Tasks;

namespace Voidwell.Auth.IdentityServer.Services.Abstractions;

public interface IIdentityProviderEventService
{
    Task RaiseUserLoginSuccessAsync(string username, string subjectId, string name, string clientId = null);

    Task RaiseUserLoginFailureAsync(string username, string error, string clientId = null);

    Task RaiseUserLogoutSuccessAsync(string subjectId, string username);
}