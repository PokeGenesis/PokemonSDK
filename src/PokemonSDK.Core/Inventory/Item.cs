namespace PokemonSDK.Core.Inventory;

/// <summary>
/// Represents an item that can be found, bought, or used
/// </summary>
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public int Price { get; set; }
    public bool IsKeyItem { get; set; }
    public bool IsConsumable { get; set; } = true;
    
    // Usage context
    public bool CanUseInBattle { get; set; }
    public bool CanUseInField { get; set; }
    
    // Effects
    public Dictionary<string, object> Effects { get; set; } = new();
}

/// <summary>
/// Represents different types of items
/// </summary>
public enum ItemType
{
    Medicine = 0,
    PokeBall = 1,
    BattleItem = 2,
    Berry = 3,
    Mail = 4,
    HeldItem = 5,
    EvolutionStone = 6,
    KeyItem = 7,
    TMsHMs = 8,
    Misc = 9
}
