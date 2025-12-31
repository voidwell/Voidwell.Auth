using System;

namespace Voidwell.Auth.UserManagement.Models;

public class SimpleUser
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
}
