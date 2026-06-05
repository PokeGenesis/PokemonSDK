namespace SDK.MonoGame.Tests;

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Moq;
using SDK.Core.Entities;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.Services;
using SDK.MonoGame.Input;
using SDK.MonoGame.World;

public class HeadlessSmokeTests
{
    private static IServiceProvider BuildHeadlessServices()
    {
        var mockEncounters = new Mock<IEncounterSystem>();
        mockEncounters
            .Setup(e => e.GetZonesByIdentifier(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(new List<EncounterZone>());

        var services = new ServiceCollection();
        services.AddSingleton<IGameClock, GameTimeClock>();
        services.AddSingleton<IWeatherSystem, WeatherSystem>();
        services.AddSingleton<IEncounterSystem>(_ => mockEncounters.Object);
        services.AddSingleton<WorldSystem>();
        services.AddSingleton<PlayerSystem>();
        services.AddSingleton<IInputProvider, NullInputProvider>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public void Run_60Frames_NoException()
    {
        var sp = BuildHeadlessServices();
        var act = () => HeadlessRunner.Run(sp, 60);
        act.Should().NotThrow();
    }

    [Fact]
    public void Run_10Frames_PlayerPositionUnchanged_WithNullInput()
    {
        var sp = BuildHeadlessServices();
        var player = sp.GetRequiredService<PlayerSystem>();
        var before = player.Position;
        HeadlessRunner.Run(sp, 10);
        player.Position.Should().Be(before);
    }

    [Fact]
    public void Run_1Frame_ClockReturnsValidTimeOfDay()
    {
        var sp = BuildHeadlessServices();
        HeadlessRunner.Run(sp, 1);
        var clock = sp.GetRequiredService<IGameClock>();
        var validValues = new[] { TimeOfDay.Morning, TimeOfDay.Day, TimeOfDay.Evening, TimeOfDay.Night };
        validValues.Should().Contain(clock.GetTimeOfDay());
    }
}
