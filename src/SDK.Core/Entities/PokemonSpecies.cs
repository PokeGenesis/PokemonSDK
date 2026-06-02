namespace SDK.Core.Entities;

public class PokemonSpecies
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int Generation { get; set; }
    public string? OriginRegion { get; set; }
    public int Type1Id { get; set; }
    public int? Type2Id { get; set; }
}
