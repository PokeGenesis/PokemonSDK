namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Identifier).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Generation).IsRequired();
        builder.Property(e => e.Accuracy).IsRequired();
        builder.Property(e => e.PP).IsRequired();
        builder.Property(e => e.Category).HasConversion<int>().IsRequired();
        builder.Property(e => e.Power).IsRequired(false);
        builder.HasOne(e => e.Type).WithMany().HasForeignKey(e => e.TypeId).IsRequired();
        builder.HasIndex(e => e.Generation);
    }
}
