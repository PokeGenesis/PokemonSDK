namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Locale).IsRequired().HasMaxLength(10);
        builder.Property(e => e.Field).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Value).IsRequired();
        // D-07 — collision = erreur, jamais override silencieux
        builder.HasIndex(e => new { e.EntityType, e.EntityId, e.Locale, e.Field })
               .IsUnique();
    }
}
