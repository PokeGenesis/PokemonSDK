namespace SDK.Plugins.Nuzlocke;

using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;

public sealed class NuzlockePlugin : IBattlePlugin
{
    private readonly Action<string, bool> _onPermanentDeath;

    public string Name => "Nuzlocke";

    public NuzlockePlugin(Action<string, bool> onPermanentDeath)
        => _onPermanentDeath = onPermanentDeath;

    public void OnPokemonFainted(BattleState state, BattlePokemon fainted)
        => _onPermanentDeath($"nuzlocke_dead_{fainted.SpeciesId}", true);

    public void OnBattleStart(BattleState state) { }
    public void OnTurnStart(BattleState state) { }
    public void OnTurnEnd(BattleState state) { }
    public void OnBattleEnd(BattleState state, BattleResult result) { }
    public void OnPokemonCaught(BattleState state, BattlePokemon caught, string zone) { }
    public void OnPokemonLevelUp(BattlePokemon pokemon, int oldLevel, int newLevel) { }
    public BattleState? OnBeforeMove(BattleState state, BattleAction action) => null;
    public BattleState? OnBeforeDamage(BattleState state, DamageResult damage) => null;
}
