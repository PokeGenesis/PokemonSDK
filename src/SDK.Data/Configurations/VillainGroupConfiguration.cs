namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class VillainGroupConfiguration : IEntityTypeConfiguration<VillainGroup>
{
    public void Configure(EntityTypeBuilder<VillainGroup> builder)
    {
        builder.ToTable("villain_groups");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Identifier).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Generation).IsRequired();
        builder.HasIndex(e => e.Identifier).IsUnique();
        builder.HasMany(e => e.Members)
               .WithOne(m => m.VillainGroup)
               .HasForeignKey(m => m.VillainGroupId);
    }
}
