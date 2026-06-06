namespace SDK.Core.Interfaces;

using SDK.Core.ValueObjects;

public interface IBattlePlugin : IPlugin
{
    // Observers — void, appelés pour notification uniquement
    void OnBattleStart(BattleState state);
    void OnTurnStart(BattleState state);
    void OnTurnEnd(BattleState state);
    void OnBattleEnd(BattleState state, BattleResult result);
    void OnPokemonFainted(BattleState state, BattlePokemon fainted);

    // Stubs — contrat NuGet stable, non appelés par BattleEngine Phase 5
    // (EncounterSystem non câblé à BattleEngine avant Phase 6+)
    void OnPokemonCaught(BattleState state, BattlePokemon caught, string zone);
    void OnPokemonLevelUp(BattlePokemon pokemon, int oldLevel, int newLevel);

    // Chain state — null = pas de modification, non-null remplace l'état
    BattleState? OnBeforeMove(BattleState state, BattleAction action);
    BattleState? OnBeforeDamage(BattleState state, DamageResult damage);
}
