namespace SDK.Plugins.Turbo.Tests;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class TurboPluginTests
{
    [Fact]
    public void Default_IsActive_True_TextSpeed_MaxValue()
    {
        var plugin = new TurboPlugin();

        plugin.IsActive.Should().BeTrue();
        plugin.TextSpeedMultiplier.Should().Be(float.MaxValue);
    }

    [Fact]
    public void Disabled_IsActive_False_TextSpeed_Normal()
    {
        var plugin = new TurboPlugin(isActive: false);

        plugin.IsActive.Should().BeFalse();
        plugin.TextSpeedMultiplier.Should().Be(1.0f);
    }

    [Fact]
    public void Custom_Multiplier_Preserved_When_Active()
    {
        var plugin = new TurboPlugin(isActive: true, textSpeedMultiplier: 3.0f);

        plugin.TextSpeedMultiplier.Should().Be(3.0f);
    }

    [Fact]
    public void Name_Is_Turbo()
    {
        new TurboPlugin().Name.Should().Be("Turbo");
    }

    [Fact]
    public void Lifecycle_Hooks_Do_Not_Throw()
    {
        var plugin = new TurboPlugin();
        var move = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);
        var pokemon = new BattlePokemon(1, "Bulbasaur", 50, 100, 100, 50, 50, 50, 50, 50,
            1, null, new[] { move });
        var state = new BattleState(pokemon, pokemon, 1, WeatherType.None,
            new BattleConfig(CritEnabled: false), Array.Empty<string>());
        var result = new BattleResult(true, 3, "Normal");
        var action = new BattleAction(move.MoveId, true);
        var damage = new DamageResult(42, false, 1.0m);

        var act = () =>
        {
            plugin.OnBattleStart(state);
            plugin.OnTurnStart(state);
            plugin.OnTurnEnd(state);
            plugin.OnBattleEnd(state, result);
            plugin.OnPokemonFainted(state, pokemon);
            plugin.OnPokemonCaught(state, pokemon, "route-1");
            plugin.OnPokemonLevelUp(pokemon, 10, 11);
            plugin.OnBeforeMove(state, action);
            plugin.OnBeforeDamage(state, damage);
        };

        act.Should().NotThrow();
    }
}
