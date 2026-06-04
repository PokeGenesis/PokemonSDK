namespace SDK.Battle.Formulas;

using SDK.Core.ValueObjects;

public interface IDamageFormula
{
    int Generation { get; }
    DamageResult Calculate(
        BattlePokemon attacker,
        BattlePokemon defender,
        BattleMove move,
        decimal typeMultiplier,
        BattleConfig config);
}
