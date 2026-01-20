namespace PokemonSDK.Core.Localization;

/// <summary>
/// Supported languages
/// </summary>
public enum Language
{
    English = 0,
    French = 1,
    Spanish = 2
}

/// <summary>
/// Localized text for multi-language support
/// </summary>
public class LocalizedText
{
    public int Id { get; set; }
    public string English { get; set; } = string.Empty;
    public string French { get; set; } = string.Empty;
    public string Spanish { get; set; } = string.Empty;
    
    public string GetText(Language language) => language switch
    {
        Language.English => English,
        Language.French => French,
        Language.Spanish => Spanish,
        _ => English
    };
}
