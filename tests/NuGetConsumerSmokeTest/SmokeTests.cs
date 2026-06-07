using Xunit;

namespace NuGetConsumerSmokeTest;

public class PackagePresenceTests
{
    [Fact]
    public void SDK_Core_resolves() =>
        Assert.NotNull(typeof(SDK.Core.Entities.PokemonSpecies));

    [Fact]
    public void SDK_Data_resolves() =>
        Assert.NotNull(typeof(SDK.Data.PokemonDbContext));

    [Fact]
    public void SDK_Battle_resolves() =>
        Assert.NotNull(typeof(SDK.Battle.BattleEngine));

    [Fact]
    public void SDK_Scripting_resolves() =>
        Assert.NotNull(typeof(SDK.Scripting.Engine.LuaScriptEngine));

    [Fact]
    public void SDK_Plugins_Nuzlocke_resolves() =>
        Assert.NotNull(typeof(SDK.Plugins.Nuzlocke.NuzlockePlugin));

    [Fact]
    public void SDK_Plugins_Randomizer_resolves() =>
        Assert.NotNull(typeof(SDK.Plugins.Randomizer.RandomizerPlugin));

    [Fact]
    public void SDK_Plugins_Turbo_resolves() =>
        Assert.NotNull(typeof(SDK.Plugins.Turbo.TurboPlugin));
}
