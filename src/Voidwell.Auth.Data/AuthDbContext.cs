using Microsoft.EntityFrameworkCore;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Profile>()
                .HasIndex(a => a.DisplayName)
                .IsUnique();
            modelBuilder.Entity<Profile>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Authentication>();
            modelBuilder.Entity<Profile>();
            modelBuilder.Entity<SecurityQuestion>()
                .HasKey(a => new { a.UserId, a.Question });

            base.OnModelCreating(modelBuilder);
        }
    }
}
