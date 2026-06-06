namespace SDK.Plugins.Turbo;

using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;

// Marqueur de mode Turbo. Les renderers MonoGame vérifient IsActive
// pour accélérer animations/transitions, et TextSpeedMultiplier pour
// le défilement de tous les dialogues (battle + NPC + menus).
public sealed class TurboPlugin : IBattlePlugin
{
    public string Name => "Turbo";
    public bool IsActive { get; }

    // Multiplicateur de vitesse de texte pour TOUS les dialogues.
    // float.MaxValue = texte instantané. 1.0f = vitesse normale.
    public float TextSpeedMultiplier { get; }

    public TurboPlugin(bool isActive = true, float textSpeedMultiplier = float.MaxValue)
    {
        IsActive = isActive;
        TextSpeedMultiplier = isActive ? textSpeedMultiplier : 1.0f;
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
