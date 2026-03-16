using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Data.EntityMappings;

internal class SecurityQuestionMapping : IEntityTypeConfiguration<SecurityQuestion>
{
    public void Configure(EntityTypeBuilder<SecurityQuestion> builder)
    {
        builder.HasKey(a => new { a.UserId, a.Question });
    }
}