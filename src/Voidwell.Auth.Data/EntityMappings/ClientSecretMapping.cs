using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Data.EntityMappings;

internal class ClientSecretMapping : IEntityTypeConfiguration<ClientSecret>
{
    public void Configure(EntityTypeBuilder<ClientSecret> builder)
    {
        builder.ToTable("ClientSecrets");
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.ClientId);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne<AuthApplication>()
            .WithMany()
            .IsRequired()
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}