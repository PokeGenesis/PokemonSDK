namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class TypeEffectivenessConfiguration : IEntityTypeConfiguration<TypeEffectiveness>
{
    public void Configure(EntityTypeBuilder<TypeEffectiveness> builder)
    {
        // Composite key includes Generation — type chart changes between gens (e.g. Fairy in Gen 6)
        builder.HasKey(e => new { e.AttackerTypeId, e.DefenderTypeId, e.Generation });
        builder.Property(e => e.DamageFactor).HasPrecision(4, 2).IsRequired();
        builder.Property(e => e.Generation).IsRequired();
    }
}
