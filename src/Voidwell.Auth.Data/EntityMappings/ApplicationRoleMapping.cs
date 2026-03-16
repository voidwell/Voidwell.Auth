using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Data.EntityMappings;

internal class ApplicationRoleMapping : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();
    }
}