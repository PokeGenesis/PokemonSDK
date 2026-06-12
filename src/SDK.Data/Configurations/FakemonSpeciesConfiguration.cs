namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class FakemonSpeciesConfiguration : IEntityTypeConfiguration<FakemonSpecies>
{
    public void Configure(EntityTypeBuilder<FakemonSpecies> builder)
    {
        builder.ToTable("fakemon_species");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Identifier).IsRequired().HasMaxLength(100);
        builder.HasIndex(e => e.Identifier).IsUnique();
        builder.Property(e => e.Generation).IsRequired();
        builder.Property(e => e.EggGroup1).IsRequired().HasMaxLength(50);
        builder.Property(e => e.EggGroup2).HasMaxLength(50);
        builder.Property(e => e.PartsManifest).HasColumnType("TEXT");

        builder.HasOne(e => e.Type1)
               .WithMany()
               .HasForeignKey(e => e.Type1Id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Type2)
               .WithMany()
               .HasForeignKey(e => e.Type2Id)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
