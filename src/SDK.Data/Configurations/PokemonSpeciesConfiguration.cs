namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;
using SDK.Core.Enums;

public class PokemonSpeciesConfiguration : IEntityTypeConfiguration<PokemonSpecies>
{
    public void Configure(EntityTypeBuilder<PokemonSpecies> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Identifier).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Generation).IsRequired();
        builder.Property(e => e.OriginRegion).HasMaxLength(100);
        builder.HasIndex(e => e.Identifier).IsUnique();

        builder.Property(p => p.BaseExpYield)
               .IsRequired()
               .HasDefaultValue(64);

        builder.Property(p => p.GrowthRate)
               .IsRequired()
               .HasConversion<int>()
               .HasDefaultValue(GrowthRate.MediumFast);
    }
}
