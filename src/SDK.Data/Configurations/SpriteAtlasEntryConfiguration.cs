namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public sealed class SpriteAtlasEntryConfiguration : IEntityTypeConfiguration<SpriteAtlasEntry>
{
    public void Configure(EntityTypeBuilder<SpriteAtlasEntry> b)
    {
        b.ToTable("sprite_atlas_entries");
        b.HasKey(e => e.Id);
        b.HasIndex(e => e.AssetKey).IsUnique();
        b.Property(e => e.AssetKey).IsRequired().HasMaxLength(100);
        b.Property(e => e.View).IsRequired().HasMaxLength(20);
        b.Property(e => e.AtlasPath).IsRequired().HasMaxLength(260);
    }
}
