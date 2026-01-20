namespace PokemonSDK.Core.Models;

/// <summary>
/// Represents an ability that affects Pokemon behavior in and out of battle
/// </summary>
public class Ability
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
    
    // Ability effects are handled by the battle engine
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
