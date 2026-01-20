using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Models;

/// <summary>
/// Represents an individual Pokemon instance with its current state
/// </summary>
public class Pokemon
{
    public Guid Id { get; set; }
    public int SpeciesId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public Gender Gender { get; set; }
    public Nature Nature { get; set; }
    public int AbilityId { get; set; }
    public bool IsShiny { get; set; }
    
    // IVs (Individual Values) - 0 to 31
    public int HP_IV { get; set; }
    public int Attack_IV { get; set; }
    public int Defense_IV { get; set; }
    public int SpecialAttack_IV { get; set; }
    public int SpecialDefense_IV { get; set; }
    public int Speed_IV { get; set; }
    
    // EVs (Effort Values) - 0 to 252 per stat, max 510 total
    public int HP_EV { get; set; }
    public int Attack_EV { get; set; }
    public int Defense_EV { get; set; }
    public int SpecialAttack_EV { get; set; }
    public int SpecialDefense_EV { get; set; }
    public int Speed_EV { get; set; }
    
    // Current battle state
    public int CurrentHP { get; set; }
    public StatusCondition Status { get; set; }
    public int Friendship { get; set; } = 70;
    
    // Moves (up to 4)
    public List<PokemonMove> Moves { get; set; } = new();
    
    // Item
    public int? HeldItemId { get; set; }
    
    // Trainer info
    public int OriginalTrainerId { get; set; }
    public string OriginalTrainerName { get; set; } = string.Empty;
    
    public Pokemon()
    {
        Id = Guid.NewGuid();
    }
    
    /// <summary>
    /// Calculate a stat value based on base stat, IV, EV, level, and nature
    /// </summary>
    public int CalculateStat(Stat stat, PokemonSpecies species)
    {
        var baseStat = species.GetBaseStat(stat);
        var iv = GetIV(stat);
        var ev = GetEV(stat);
        
        if (stat == Stat.HP)
        {
            // HP calculation formula
            return (2 * baseStat + iv + ev / 4) * Level / 100 + Level + 10;
        }
        else
        {
            // Other stats calculation formula
            var statValue = (2 * baseStat + iv + ev / 4) * Level / 100 + 5;
            var natureMultiplier = GetNatureMultiplier(stat);
            return (int)(statValue * natureMultiplier);
        }
    }
    
    private int GetIV(Stat stat) => stat switch
    {
        Stat.HP => HP_IV,
        Stat.Attack => Attack_IV,
        Stat.Defense => Defense_IV,
        Stat.SpecialAttack => SpecialAttack_IV,
        Stat.SpecialDefense => SpecialDefense_IV,
        Stat.Speed => Speed_IV,
        _ => 0
    };
    
    private int GetEV(Stat stat) => stat switch
    {
        Stat.HP => HP_EV,
        Stat.Attack => Attack_EV,
        Stat.Defense => Defense_EV,
        Stat.SpecialAttack => SpecialAttack_EV,
        Stat.SpecialDefense => SpecialDefense_EV,
        Stat.Speed => Speed_EV,
        _ => 0
    };
    
    private double GetNatureMultiplier(Stat stat)
    {
        // Nature effects: some natures increase one stat by 10% and decrease another by 10%
        var (increased, decreased) = Nature switch
        {
            Nature.Lonely => (Stat.Attack, Stat.Defense),
            Nature.Brave => (Stat.Attack, Stat.Speed),
            Nature.Adamant => (Stat.Attack, Stat.SpecialAttack),
            Nature.Naughty => (Stat.Attack, Stat.SpecialDefense),
            Nature.Bold => (Stat.Defense, Stat.Attack),
            Nature.Relaxed => (Stat.Defense, Stat.Speed),
            Nature.Impish => (Stat.Defense, Stat.SpecialAttack),
            Nature.Lax => (Stat.Defense, Stat.SpecialDefense),
            Nature.Timid => (Stat.Speed, Stat.Attack),
            Nature.Hasty => (Stat.Speed, Stat.Defense),
            Nature.Jolly => (Stat.Speed, Stat.SpecialAttack),
            Nature.Naive => (Stat.Speed, Stat.SpecialDefense),
            Nature.Modest => (Stat.SpecialAttack, Stat.Attack),
            Nature.Mild => (Stat.SpecialAttack, Stat.Defense),
            Nature.Quiet => (Stat.SpecialAttack, Stat.Speed),
            Nature.Rash => (Stat.SpecialAttack, Stat.SpecialDefense),
            Nature.Calm => (Stat.SpecialDefense, Stat.Attack),
            Nature.Gentle => (Stat.SpecialDefense, Stat.Defense),
            Nature.Sassy => (Stat.SpecialDefense, Stat.Speed),
            Nature.Careful => (Stat.SpecialDefense, Stat.SpecialAttack),
            _ => (Stat.HP, Stat.HP) // Neutral natures
        };
        
        if (stat == increased) return 1.1;
        if (stat == decreased) return 0.9;
        return 1.0;
    }
}

/// <summary>
/// Represents a move that a Pokemon knows
/// </summary>
public class PokemonMove
{
    public int MoveId { get; set; }
    public int CurrentPP { get; set; }
    public int MaxPP { get; set; }
}
