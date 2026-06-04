namespace SDK.Core.Interfaces;

using SDK.Core.Entities;

public interface IEncounterSystem
{
    IEnumerable<EncounterZone> GetZones(int generation);
}
