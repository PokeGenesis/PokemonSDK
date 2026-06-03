namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class AbilityConfiguration : IEntityTypeConfiguration<Ability>
{
    public void Configure(EntityTypeBuilder<Ability> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Identifier).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Generation).IsRequired();
        builder.HasIndex(e => e.Identifier).IsUnique();
    }
}
