namespace SDK.Core.Entities;

public class PokemonForm
{
    public int Id { get; set; }
    public int SpeciesId { get; set; }
    public string? FormKey { get; set; }
    public string AssetKey { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public int Generation { get; set; }
}
