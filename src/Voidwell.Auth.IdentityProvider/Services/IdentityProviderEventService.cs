using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Voidwell.Auth.IdentityProvider.Services.Abstractions;

namespace Voidwell.Auth.IdentityProvider.Services;

public class IdentityProviderEventService : IIdentityProviderEventService
{
    private readonly IEventService _eventService;

    public IdentityProviderEventService(IEventService eventService)
    {
        _eventService = eventService;
    }

    public Task RaiseUserLoginSuccessAsync(string username, string subjectId, string name, string clientId = null)
    {
        return _eventService.RaiseAsync(new UserLoginSuccessEvent(username, subjectId, name, clientId: clientId));
    }

    public Task RaiseUserLoginFailureAsync(string username, string error, string clientId = null)
    {
        return _eventService.RaiseAsync(new UserLoginFailureEvent(username, error, clientId: clientId));
    }

    public Task RaiseUserLogoutSuccessAsync(string subjectId, string username)
    {
        return _eventService.RaiseAsync(new UserLogoutSuccessEvent(subjectId, username));
    }
}