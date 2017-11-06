using Microsoft.AspNetCore.Identity;
using System;

namespace Voidwell.Auth.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string PasswordSalt { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTimeOffset? PasswordSetDate { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string TimeZone { get; set; }
    }
}
