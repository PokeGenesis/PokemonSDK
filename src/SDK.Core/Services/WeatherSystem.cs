namespace SDK.Core.Services;

using SDK.Core.Enums;
using SDK.Core.Interfaces;

public class WeatherSystem : IWeatherSystem
{
    public WeatherType GetWeather(BiomeType biome, TimeOfDay time) =>
        (biome, time) switch
        {
            (BiomeType.Cave, _)                  => WeatherType.None,
            (BiomeType.Building, _)              => WeatherType.None,
            (BiomeType.Water, TimeOfDay.Night)   => WeatherType.Rain,
            (BiomeType.Water, TimeOfDay.Evening) => WeatherType.Rain,
            _                                    => WeatherType.None,
        };
}
