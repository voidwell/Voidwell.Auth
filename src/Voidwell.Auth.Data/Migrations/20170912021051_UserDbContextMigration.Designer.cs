using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Voidwell.VoidwellAuth.Data.DBContext;

namespace Voidwell.VoidwellAuth.Data.Migrations
{
    [DbContext(typeof(UserDbContext))]
    [Migration("20170912021051_UserDbContextMigration")]
    partial class UserDbContextMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Voidwell.VoidwellAuth.Data.Models.Authentication", b =>
                {
                    b.Property<Guid>("AuthenticationId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("LastLoginDate");

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("PasswordSalt")
                        .IsRequired();

                    b.Property<DateTimeOffset?>("PasswordSetDate");

                    b.Property<Guid>("UserId");

                    b.HasKey("AuthenticationId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Authentication");
                });

            modelBuilder.Entity("Voidwell.VoidwellAuth.Data.Models.Profile", b =>
                {
                    b.Property<Guid>("ProfileId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("TimeZone");

                    b.Property<Guid>("UserId");

                    b.HasKey("ProfileId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Profile");
                });

            modelBuilder.Entity("Voidwell.VoidwellAuth.Data.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("Banned");

                    b.Property<DateTimeOffset?>("Created");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Voidwell.VoidwellAuth.Data.Models.Authentication", b =>
                {
                    b.HasOne("Voidwell.VoidwellAuth.Data.Models.User", "User")
                        .WithOne("Authentication")
                        .HasForeignKey("Voidwell.VoidwellAuth.Data.Models.Authentication", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Voidwell.VoidwellAuth.Data.Models.Profile", b =>
                {
                    b.HasOne("Voidwell.VoidwellAuth.Data.Models.User", "User")
                        .WithOne("Profile")
                        .HasForeignKey("Voidwell.VoidwellAuth.Data.Models.Profile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
