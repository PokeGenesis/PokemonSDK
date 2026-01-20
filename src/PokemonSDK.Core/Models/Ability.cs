namespace PokemonSDK.Core.Models;

/// <summary>
/// Represents an ability that affects Pokemon behavior in and out of battle
/// </summary>
public class Ability
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Ability effects are handled by the battle engine
    public Dictionary<string, object> Effects { get; set; } = new();
}
