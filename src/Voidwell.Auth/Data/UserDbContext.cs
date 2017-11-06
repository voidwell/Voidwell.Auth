using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Voidwell.Auth.Models;
using System;

namespace Voidwell.Auth.Data
{
    public class UserDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(a =>
            {
                a.Property(u => u.Id).HasDefaultValue(Guid.NewGuid());
            });

            builder.Entity<ApplicationRole>(a =>
            {
                a.Property(u => u.Id).HasDefaultValue(Guid.NewGuid());
            });
        }
    }
}
