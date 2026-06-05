namespace SDK.Core.Interfaces;

using SDK.Core.Enums;

public interface IWeatherSystem
{
    WeatherType GetWeather(BiomeType biome, TimeOfDay time);
}
