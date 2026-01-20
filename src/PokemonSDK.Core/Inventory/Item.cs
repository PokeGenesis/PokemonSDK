namespace PokemonSDK.Core.Inventory;

/// <summary>
/// Represents an item that can be found, bought, or used
/// </summary>
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Multi-language names
    public string NameFrench { get; set; } = string.Empty;
    public string NameSpanish { get; set; } = string.Empty;
    
    // Multi-language descriptions
    public string Description { get; set; } = string.Empty;
    public string DescriptionFrench { get; set; } = string.Empty;
    public string DescriptionSpanish { get; set; } = string.Empty;
    
    // Filter option
    public bool IsEnabled { get; set; } = true;
    
    public ItemType Type { get; set; }
    public int Price { get; set; }
    public bool IsKeyItem { get; set; }
    public bool IsConsumable { get; set; } = true;
    
    // Usage context
    public bool CanUseInBattle { get; set; }
    public bool CanUseInField { get; set; }
    
    // Effects
    public Dictionary<string, object> Effects { get; set; } = new();
    
    /// <summary>
    /// Get localized name
    /// </summary>
    public string GetLocalizedName(Localization.Language language) => language switch
    {
        Localization.Language.French => !string.IsNullOrEmpty(NameFrench) ? NameFrench : Name,
        Localization.Language.Spanish => !string.IsNullOrEmpty(NameSpanish) ? NameSpanish : Name,
        _ => Name
    };
    
    /// <summary>
    /// Get localized description
    /// </summary>
    public string GetLocalizedDescription(Localization.Language language) => language switch
    {
        Localization.Language.French => !string.IsNullOrEmpty(DescriptionFrench) ? DescriptionFrench : Description,
        Localization.Language.Spanish => !string.IsNullOrEmpty(DescriptionSpanish) ? DescriptionSpanish : Description,
        _ => Description
    };
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
