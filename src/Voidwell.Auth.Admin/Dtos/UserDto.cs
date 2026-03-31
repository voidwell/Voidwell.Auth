using System;
using System.Collections.Generic;

namespace Voidwell.Auth.Admin.Dtos;

public class UserDto
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public string TimeZone { get; set; }

    public DateTimeOffset? CreatedDate { get; set; }

    public DateTimeOffset? LastUpdatedDate { get; set; }

    public DateTimeOffset? LastLoginDate { get; set; }

    public DateTimeOffset? PasswordSetDate { get; set; }

    public DateTimeOffset? LockoutEndDate { get; set; }

    public bool LockoutEnabled { get; set; }

    public IEnumerable<string> Roles { get; set; }
}
