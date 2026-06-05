namespace SDK.MonoGame.World;

using SDK.Core.Enums;
using SDK.Core.Interfaces;

public class WorldSystem
{
    private readonly IGameClock       _clock;
    private readonly IEncounterSystem _encounters;
    private readonly IWeatherSystem   _weather;

    public WeatherType CurrentWeather { get; private set; } = WeatherType.None;
    public string      CurrentZoneId  { get; set; }         = "test-zone";
    public int         CurrentGen     { get; set; }         = 1;

    public WorldSystem(IGameClock clock, IEncounterSystem encounters, IWeatherSystem weather)
    {
        _clock      = clock;
        _encounters = encounters;
        _weather    = weather;
    }

    public void Update(TimeSpan delta)
    {
        _clock.Update(delta);
        CurrentWeather = _weather.GetWeather(BiomeType.Grass, _clock.GetTimeOfDay());
    }

    public bool CheckWildEncounter()
    {
        var zones = _encounters.GetZonesByIdentifier(CurrentZoneId, CurrentGen);
        foreach (var zone in zones)
        {
            if ((float)zone.SpawnRate > Random.Shared.NextSingle() * 0.5f)
                return true;
        }
        return false;
    }
}
