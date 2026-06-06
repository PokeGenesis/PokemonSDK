namespace SDK.Plugins.Randomizer;

using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;

public sealed class RandomizerPlugin : IBattlePlugin
{
    private readonly Random _rng;
    private const int TypeCount = 18;

    public string Name => "Randomizer";

    public RandomizerPlugin(int seed) => _rng = new Random(seed);

    public BattlePokemon RandomizePokemon(BattlePokemon p)
    {
        var newType1 = _rng.Next(1, TypeCount + 1);
        var raw2 = _rng.Next(0, TypeCount + 1);
        int? newType2 = (raw2 == 0 || raw2 == newType1) ? null : raw2;
        return p with { Type1Id = newType1, Type2Id = newType2 };
    }

    public void OnBattleStart(BattleState state) { }
    public void OnTurnStart(BattleState state) { }
    public void OnTurnEnd(BattleState state) { }
    public void OnBattleEnd(BattleState state, BattleResult result) { }
    public void OnPokemonFainted(BattleState state, BattlePokemon fainted) { }
    public void OnPokemonCaught(BattleState state, BattlePokemon caught, string zone) { }
    public void OnPokemonLevelUp(BattlePokemon pokemon, int oldLevel, int newLevel) { }
    public BattleState? OnBeforeMove(BattleState state, BattleAction action) => null;
    public BattleState? OnBeforeDamage(BattleState state, DamageResult damage) => null;
}
