namespace SDK.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.Core.Entities;

public class PokemonFormConfiguration : IEntityTypeConfiguration<PokemonForm>
{
    public void Configure(EntityTypeBuilder<PokemonForm> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.AssetKey).IsRequired().HasMaxLength(200);
        builder.Property(e => e.FormKey).HasMaxLength(50);
        builder.Property(e => e.Generation).IsRequired();
    }
}
