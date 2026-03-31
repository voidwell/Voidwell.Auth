using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.Auth.Services.Abstractions;

public interface ILogoutNotificationService
{
    Task<IReadOnlyList<string>> GetFrontChannelLogoutUrisAsync(string subjectId);

    Task SendBackChannelLogoutNotificationsAsync(string subjectId);
}
