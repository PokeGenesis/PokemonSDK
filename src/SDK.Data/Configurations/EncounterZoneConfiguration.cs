namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class EncounterZoneConfiguration : IEntityTypeConfiguration<EncounterZone>
{
    public void Configure(EntityTypeBuilder<EncounterZone> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ZoneIdentifier).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Generation).IsRequired();
        builder.Property(e => e.BiomeType).HasConversion<int>();
        builder.Property(e => e.SpawnRate).HasColumnType("REAL");
        builder.HasOne<PokemonSpecies>().WithMany()
            .HasForeignKey(e => e.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
