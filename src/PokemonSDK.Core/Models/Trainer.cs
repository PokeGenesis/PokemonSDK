using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Models;

/// <summary>
/// Represents a trainer in the game
/// </summary>
public class Trainer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int Money { get; set; }
    public List<Pokemon> Party { get; set; } = new();
    public List<Pokemon> PC { get; set; } = new(); // Pokemon stored in PC
    public Inventory.Bag Bag { get; set; } = new();
    public Dictionary<int, bool> Badges { get; set; } = new(); // Badge ID -> obtained
    public Position Position { get; set; } = new();
    
    public int GetPartySize() => Party.Count;
    
    public bool HasAvailablePokemon() => Party.Any(p => p.CurrentHP > 0);
}

/// <summary>
/// Represents a position on the map
/// </summary>
public class Position
{
    public int MapId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Direction { get; set; } = "Down"; // Up, Down, Left, Right
}
