using Microsoft.AspNetCore.Identity;
using System;

namespace Voidwell.Auth.Data.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset? LastLoginDate { get; set; }

    public DateTimeOffset? PasswordSetDate { get; set; }

    public DateTimeOffset? CreatedDate { get; set; }

    public DateTimeOffset? LastUpdatedDate { get; set; }

    public DateTime? Birthdate { get; set; }

    public string TimeZone { get; set; }
}
