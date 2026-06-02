namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class PokemonBaseStatsConfiguration : IEntityTypeConfiguration<PokemonBaseStats>
{
    public void Configure(EntityTypeBuilder<PokemonBaseStats> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Hp).IsRequired();
        builder.Property(e => e.Attack).IsRequired();
        builder.Property(e => e.Defense).IsRequired();
        builder.Property(e => e.SpecialAttack).IsRequired();
        builder.Property(e => e.SpecialDefense).IsRequired();
        builder.Property(e => e.Speed).IsRequired();
    }
}
