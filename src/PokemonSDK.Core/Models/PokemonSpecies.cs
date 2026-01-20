using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Models;

/// <summary>
/// Represents a Pokemon species with its base stats and characteristics
/// </summary>
public class PokemonSpecies
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PokemonType PrimaryType { get; set; }
    public PokemonType? SecondaryType { get; set; }
    
    // Base stats
    public int BaseHP { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public int BaseSpecialAttack { get; set; }
    public int BaseSpecialDefense { get; set; }
    public int BaseSpeed { get; set; }
    
    // Other characteristics
    public List<int> PossibleAbilities { get; set; } = new();
    public int? HiddenAbility { get; set; }
    public int CatchRate { get; set; }
    public int BaseExperience { get; set; }
    public string GrowthRate { get; set; } = "MediumFast";
    public int GenderRatio { get; set; } = 50; // Percentage chance of being male
    public int EggCycles { get; set; }
    public List<string> EggGroups { get; set; } = new();
    
    // Evolution
    public List<Evolution> Evolutions { get; set; } = new();
    
    public int GetBaseStat(Stat stat) => stat switch
    {
        Stat.HP => BaseHP,
        Stat.Attack => BaseAttack,
        Stat.Defense => BaseDefense,
        Stat.SpecialAttack => BaseSpecialAttack,
        Stat.SpecialDefense => BaseSpecialDefense,
        Stat.Speed => BaseSpeed,
        _ => throw new ArgumentException($"Invalid stat: {stat}")
    };
}

/// <summary>
/// Represents an evolution possibility for a Pokemon species
/// </summary>
public class Evolution
{
    public int TargetSpeciesId { get; set; }
    public string Method { get; set; } = string.Empty; // "Level", "Item", "Trade", etc.
    public int? RequiredLevel { get; set; }
    public int? RequiredItemId { get; set; }
    public string? Condition { get; set; } // Additional conditions like "Happiness", "Time", etc.
}
