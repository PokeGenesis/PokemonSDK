namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class VillainMemberConfiguration : IEntityTypeConfiguration<VillainMember>
{
    public void Configure(EntityTypeBuilder<VillainMember> builder)
    {
        builder.ToTable("villain_members");
        builder.HasKey(e => e.Id);
    }
}
