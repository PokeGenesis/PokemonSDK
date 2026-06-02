namespace SDK.Data;

using Microsoft.EntityFrameworkCore;
using SDK.Core.Entities;

public class PokemonDbContext : DbContext
{
    public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options) { }

    public DbSet<PokemonSpecies> PokemonSpecies => Set<PokemonSpecies>();
    public DbSet<PokemonForm> PokemonForms => Set<PokemonForm>();
    public DbSet<PokemonBaseStats> PokemonBaseStats => Set<PokemonBaseStats>();
    public DbSet<Translation> Translations => Set<Translation>();
    public DbSet<PokemonType> PokemonTypes => Set<PokemonType>();
    public DbSet<TypeEffectiveness> TypeEffectiveness => Set<TypeEffectiveness>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(PokemonDbContext).Assembly);
}
