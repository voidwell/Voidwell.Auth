using Microsoft.EntityFrameworkCore;
using Voidwell.VoidwellAuth.Data.Models;

namespace Voidwell.VoidwellAuth.Data.DBContext
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Authentication>();
            modelBuilder.Entity<Profile>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            
        }
    }
}
