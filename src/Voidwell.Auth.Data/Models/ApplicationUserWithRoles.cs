using System.Collections.Generic;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Data.Models;

public class ApplicationUserWithRoles : ApplicationUser
{
    public ApplicationUserWithRoles(ApplicationUser user, HashSet<string> roles)
    {
        Id = user.Id;
        UserName = user.UserName;
        NormalizedUserName = user.NormalizedUserName;
        Email = user.Email;
        NormalizedEmail = user.NormalizedEmail;
        EmailConfirmed = user.EmailConfirmed;
        PasswordHash = user.PasswordHash;
        SecurityStamp = user.SecurityStamp;
        ConcurrencyStamp = user.ConcurrencyStamp;
        PhoneNumber = user.PhoneNumber;
        PhoneNumberConfirmed = user.PhoneNumberConfirmed;
        TwoFactorEnabled = user.TwoFactorEnabled;
        LockoutEnd = user.LockoutEnd;
        LockoutEnabled = user.LockoutEnabled;
        AccessFailedCount = user.AccessFailedCount;
        LastLoginDate = user.LastLoginDate;
        PasswordSetDate = user.PasswordSetDate;
        CreatedDate = user.CreatedDate;
        LastUpdatedDate = user.LastUpdatedDate;
        Birthdate = user.Birthdate;
        TimeZone = user.TimeZone;
        Roles = roles;
    }

    public HashSet<string> Roles { get; set; } = [];
}
