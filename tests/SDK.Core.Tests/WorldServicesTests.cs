namespace SDK.Core.Tests;

using FluentAssertions;
using SDK.Core.Enums;
using SDK.Core.Services;

public class RealTimeClockTests
{
    [Theory]
    [InlineData(0,  TimeOfDay.Night)]
    [InlineData(5,  TimeOfDay.Night)]
    [InlineData(6,  TimeOfDay.Morning)]
    [InlineData(10, TimeOfDay.Morning)]
    [InlineData(11, TimeOfDay.Day)]
    [InlineData(16, TimeOfDay.Day)]
    [InlineData(17, TimeOfDay.Evening)]
    [InlineData(20, TimeOfDay.Evening)]
    [InlineData(21, TimeOfDay.Night)]
    [InlineData(23, TimeOfDay.Night)]
    public void GetTimeOfDay_MapsHourCorrectly(int hour, TimeOfDay expected)
    {
        var clock = new RealTimeClock(() => new DateTime(2026, 6, 4, hour, 0, 0, DateTimeKind.Utc));
        clock.GetTimeOfDay().Should().Be(expected);
    }
}

public class GameTimeClockTests
{
    [Fact]
    public void Update_AccumulatesGameTime()
    {
        var sut = new GameTimeClock { Speed = 1f };
        sut.Update(TimeSpan.FromSeconds(60));
        sut.GameElapsed.Should().BeCloseTo(TimeSpan.FromMinutes(60), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetTimeOfDay_MorningAfter6GameHours()
    {
        var sut = new GameTimeClock { Speed = 60f };
        sut.Update(TimeSpan.FromSeconds(6));
        sut.GetTimeOfDay().Should().Be(TimeOfDay.Morning);
    }

    [Fact]
    public void GetTimeOfDay_WrapsAt24Hours()
    {
        var sut = new GameTimeClock { Speed = 60f };
        sut.Update(TimeSpan.FromSeconds(30));
        sut.GetTimeOfDay().Should().Be(TimeOfDay.Morning);
    }

    [Fact]
    public void SetGameTime_RestoresElapsed()
    {
        var sut = new GameTimeClock();
        var saved = TimeSpan.FromHours(14);
        sut.SetGameTime(saved);
        sut.GameElapsed.Should().Be(saved);
        sut.GetTimeOfDay().Should().Be(TimeOfDay.Day);
    }

    [Fact]
    public void Speed_ConfigurableAtRuntime()
    {
        var sut = new GameTimeClock { Speed = 1f };
        sut.Update(TimeSpan.FromSeconds(60));
        sut.Speed = 2f;
        sut.Update(TimeSpan.FromSeconds(60));
        sut.GameElapsed.Should().BeCloseTo(TimeSpan.FromMinutes(180), TimeSpan.FromSeconds(1));
    }
}

public class WeatherSystemTests
{
    private readonly WeatherSystem _sut = new();

    [Fact]
    public void Cave_AlwaysClear()
    {
        _sut.GetWeather(BiomeType.Cave, TimeOfDay.Morning).Should().Be(WeatherType.None);
        _sut.GetWeather(BiomeType.Cave, TimeOfDay.Day).Should().Be(WeatherType.None);
        _sut.GetWeather(BiomeType.Cave, TimeOfDay.Night).Should().Be(WeatherType.None);
    }

    [Fact]
    public void Water_NightAndEvening_Rain()
    {
        _sut.GetWeather(BiomeType.Water, TimeOfDay.Night).Should().Be(WeatherType.Rain);
        _sut.GetWeather(BiomeType.Water, TimeOfDay.Evening).Should().Be(WeatherType.Rain);
    }

    [Fact]
    public void Grass_Day_Clear()
        => _sut.GetWeather(BiomeType.Grass, TimeOfDay.Day).Should().Be(WeatherType.None);

    [Fact]
    public void Route_Morning_Clear()
        => _sut.GetWeather(BiomeType.Route, TimeOfDay.Morning).Should().Be(WeatherType.None);

    [Fact]
    public void Building_AlwaysClear()
    {
        _sut.GetWeather(BiomeType.Building, TimeOfDay.Night).Should().Be(WeatherType.None);
        _sut.GetWeather(BiomeType.Building, TimeOfDay.Day).Should().Be(WeatherType.None);
    }
}
