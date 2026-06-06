namespace SDK.Plugins.Randomizer.Tests;

using SDK.Battle.Plugins;
using SDK.Core.Enums;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class RandomizerPluginTests
{
    private static BattlePokemon MakePokemon() =>
        new BattlePokemon(1, "TestMon", 50, 100, 100, 50, 50, 50, 50, 50,
            1, null, new[] { new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35) });

    [Fact]
    public void RandomizePokemon_SameSeed_ProducesSameTypes()
    {
        var p = MakePokemon();
        var r1 = new RandomizerPlugin(42).RandomizePokemon(p);
        var r2 = new RandomizerPlugin(42).RandomizePokemon(p);

        r1.Type1Id.Should().Be(r2.Type1Id);
        r1.Type2Id.Should().Be(r2.Type2Id);
    }

    [Fact]
    public void RandomizePokemon_Type1Id_InValidRange()
    {
        var result = new RandomizerPlugin(99).RandomizePokemon(MakePokemon());

        result.Type1Id.Should().BeInRange(1, 18);
    }

    [Fact]
    public void RandomizerPlugin_RegistersInPluginRegistry_NoException()
    {
        var registry = new PluginRegistry();
        var act = () => registry.Register(new RandomizerPlugin(1));

        act.Should().NotThrow();
    }
}
