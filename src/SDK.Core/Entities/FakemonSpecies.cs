namespace SDK.Core.Entities;

public class FakemonSpecies
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int Generation { get; set; }
    public int BaseHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public int BaseSpecialAtk { get; set; }
    public int BaseSpecialDef { get; set; }
    public int BaseSpeed { get; set; }
    public int Type1Id { get; set; }
    public int? Type2Id { get; set; }
    public string EggGroup1 { get; set; } = string.Empty;
    public string? EggGroup2 { get; set; }
    public bool IsLegendary { get; set; }
    public string? PartsManifest { get; set; }
    public PokemonType? Type1 { get; set; }
    public PokemonType? Type2 { get; set; }
}
