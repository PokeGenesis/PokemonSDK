---
sidebar_position: 4
---

# PokeForge.SDK.Battle

Headless 1v1 battle engine with immutable state and pluggable formulas.

```bash
dotnet add package PokeForge.SDK.Battle
```

## Quick start

```csharp
var formula = new StandardDamageFormula();
var difficulty = new NormalDifficultyMode();
var engine = new BattleEngine(formula, difficulty);

var state = BattleState.Start(playerTeam, rivalTeam);
while (!state.IsOver)
{
    var move = ChooseMove(state); // your input logic
    state = engine.ExecuteTurn(state, move);
}
```

## BattleState

`BattleState` is an **immutable record**. Every `ExecuteTurn` call returns a new state; the old one is never mutated. This makes replays, undo, and test assertions trivial.

```csharp
// Snapshot HP before and after
var before = state.Combatants[0].CurrentHp;
var next = engine.ExecuteTurn(state, move);
var after = next.Combatants[0].CurrentHp;

int damage = before - after;
```

## Pluggable damage formula

Implement `IDamageFormula` to replace the built-in calculation:

```csharp
public class CritAlwaysFormula : IDamageFormula
{
    private readonly StandardDamageFormula _base = new();

    public int Calculate(DamageContext ctx)
    {
        var boosted = ctx with { IsCritical = true };
        return _base.Calculate(boosted);
    }
}

var engine = new BattleEngine(new CritAlwaysFormula(), difficulty);
```

## Difficulty modes

| Mode | Behavior |
|------|---------|
| `NormalDifficultyMode` | AI picks moves with mild type-effectiveness weighting |
| `HardDifficultyMode` | AI always picks the highest-damage available move |

Implement `IDifficultyMode` for custom AI.

## Status conditions

Sleep and Freeze **do not skip the attacker's turn** — the immobilized Pokémon loses its action for that turn, but the turn counter still advances. This matches later-generation behavior.

```csharp
var status = state.Combatants[1].StatusCondition;
// StatusCondition.Sleep | Freeze | Burn | Paralysis | Poison | None
```

## Battle plugins

Register `IBattlePlugin` implementations to intercept battle events:

```csharp
var engine = new BattleEngine(formula, difficulty,
    plugins: new IBattlePlugin[] { new NuzlockePlugin() });
```

See [SDK.Plugins](plugins) for bundled plugins.
