using Microsoft.EntityFrameworkCore;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.Inventory;
using PokemonSDK.Core.Localization;

namespace PokemonSDK.Core.Database;

/// <summary>
/// Database context for PokemonSDK
/// </summary>
public class PokemonDbContext : DbContext
{
    public DbSet<PokemonSpecies> Species { get; set; } = null!;
    public DbSet<Move> Moves { get; set; } = null!;
    public DbSet<Ability> Abilities { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Pokemon> Pokemon { get; set; } = null!;
    public DbSet<Trainer> Trainers { get; set; } = null!;
    public DbSet<LocalizedText> LocalizedTexts { get; set; } = null!;
    public DbSet<DataFilterOptions> FilterOptions { get; set; } = null!;
    
    public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure PokemonSpecies
        modelBuilder.Entity<PokemonSpecies>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.HasIndex(e => e.Name);
        });
        
        // Configure Move
        modelBuilder.Entity<Move>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.HasIndex(e => e.Name);
        });
        
        // Configure Ability
        modelBuilder.Entity<Ability>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.HasIndex(e => e.Name);
        });
        
        // Configure Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.HasIndex(e => e.Name);
        });
        
        // Configure Pokemon
        modelBuilder.Entity<Pokemon>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nickname).HasMaxLength(100);
        });
        
        // Configure Trainer
        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasMany(e => e.Party).WithOne().HasForeignKey("TrainerId");
        });
        
        // Configure LocalizedText
        modelBuilder.Entity<LocalizedText>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.English).HasMaxLength(1000);
            entity.Property(e => e.French).HasMaxLength(1000);
            entity.Property(e => e.Spanish).HasMaxLength(1000);
        });
        
        // Configure DataFilterOptions
        modelBuilder.Entity<DataFilterOptions>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
        });
    }
}

/// <summary>
/// Filter options for data
/// </summary>
public class DataFilterOptions
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty; // "Species", "Moves", "Abilities", "Items"
    public bool EnabledOnly { get; set; } = true;
    public string? TypeFilter { get; set; }
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
    public string? Generation { get; set; }
}
