using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Models;

/// <summary>
/// Represents a move that Pokemon can learn and use in battle
/// </summary>
public class Move
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PokemonType Type { get; set; }
    public MoveCategory Category { get; set; }
    public int Power { get; set; }
    public int Accuracy { get; set; }
    public int PP { get; set; }
    public int Priority { get; set; }
    
    // Target
    public string Target { get; set; } = "Single"; // Single, AllOpponents, AllAllies, Self, etc.
    
    // Effects
    public List<MoveEffect> Effects { get; set; } = new();
    
    // Flags
    public bool MakesContact { get; set; }
    public bool CanBeProtected { get; set; } = true;
    public bool CanBeMirrored { get; set; } = true;
    public bool CanBeSnatched { get; set; }
    public bool IsSoundBased { get; set; }
    public bool IsPunchMove { get; set; }
    public bool IsBiteMove { get; set; }
}

/// <summary>
/// Represents an effect that a move can have
/// </summary>
public class MoveEffect
{
    public string Type { get; set; } = string.Empty; // "Damage", "Status", "StatChange", "Heal", etc.
    public string? Target { get; set; } // Which target gets the effect
    public int Chance { get; set; } = 100; // Percentage chance of effect occurring
    public Dictionary<string, object> Parameters { get; set; } = new();
}
