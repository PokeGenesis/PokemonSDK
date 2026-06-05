namespace SDK.Data.Services;

using SDK.Core.Entities;
using SDK.Core.Interfaces;

public class EncounterSystem(PokemonDbContext ctx) : IEncounterSystem
{
    public IEnumerable<EncounterZone> GetZones(int generation)
        => ctx.EncounterZones.Where(e => e.Generation == generation).ToList();

    public IEnumerable<EncounterZone> GetZonesByIdentifier(string zoneIdentifier, int generation)
        => ctx.EncounterZones
              .Where(e => e.ZoneIdentifier == zoneIdentifier && e.Generation == generation)
              .ToList();
}
