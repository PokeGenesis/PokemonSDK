namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Identifier).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Generation).IsRequired();
        builder.HasIndex(e => e.Identifier).IsUnique();
        builder.HasOne(e => e.GymLeader)
               .WithMany(t => t.Badges)
               .HasForeignKey(e => e.GymLeaderId);
    }
}
