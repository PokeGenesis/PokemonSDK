namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("characters");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Identifier).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Role).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Generation).IsRequired();
        builder.HasIndex(e => e.Identifier).IsUnique();
        builder.HasMany(e => e.VillainMemberships)
               .WithOne(m => m.Character)
               .HasForeignKey(m => m.CharacterId);
    }
}
