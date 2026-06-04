namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class LearnsetConfiguration : IEntityTypeConfiguration<Learnset>
{
    public void Configure(EntityTypeBuilder<Learnset> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Generation).IsRequired();
        builder.Property(e => e.LearnLevel).IsRequired();
        builder.HasOne(e => e.Species).WithMany().HasForeignKey(e => e.SpeciesId).IsRequired();
        builder.HasOne(e => e.Move).WithMany().HasForeignKey(e => e.MoveId).IsRequired();
        builder.HasIndex(e => new { e.SpeciesId, e.Generation });
    }
}
