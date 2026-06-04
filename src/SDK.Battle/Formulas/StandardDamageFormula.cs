namespace SDK.Battle.Formulas;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;

public sealed class StandardDamageFormula : IDamageFormula
{
    public int Generation => 4;

    public DamageResult Calculate(
        BattlePokemon attacker,
        BattlePokemon defender,
        BattleMove move,
        decimal typeMultiplier,
        BattleConfig config)
    {
        if (move.Power is null || move.Category == MoveCategory.Status)
            return new DamageResult(0, false, typeMultiplier);

        // Gen 4+: Physical → Atk/Def, Special → SpAtk/SpDef
        int a = move.Category == MoveCategory.Physical ? attacker.Attack : attacker.SpecialAttack;
        int d = move.Category == MoveCategory.Physical ? defender.Defense : defender.SpecialDefense;

        if (d == 0) d = 1;

        double baseDamage = (2.0 * attacker.Level / 5.0 + 2.0) * move.Power.Value * a / d / 50.0 + 2.0;

        bool isCritical = config.CritEnabled && Random.Shared.NextDouble() < 1.0 / 24.0;
        double critMod = isCritical ? 1.5 : 1.0;
        double randomFactor = Random.Shared.Next(217, 256) / 255.0;

        int damage = Math.Max(1, (int)(baseDamage * (double)typeMultiplier * critMod * randomFactor));
        return new DamageResult(damage, isCritical, typeMultiplier);
    }
}
