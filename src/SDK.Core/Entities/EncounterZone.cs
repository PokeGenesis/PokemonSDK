namespace SDK.Core.Entities;

using SDK.Core.Enums;

public class EncounterZone
{
    public int Id { get; set; }
    public string ZoneIdentifier { get; set; } = string.Empty;
    public int Generation { get; set; }
    public BiomeType BiomeType { get; set; }
    public int SpeciesId { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public decimal SpawnRate { get; set; }
}
