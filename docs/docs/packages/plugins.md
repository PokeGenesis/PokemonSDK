---
sidebar_position: 8
---

# PokeForge.SDK.Plugins

Battle plugins — Nuzlocke, Randomizer, and Turbo. All implement `IBattlePlugin` from `SDK.Core`.

```bash
dotnet add package PokeForge.SDK.Plugins
```

## Register plugins

```csharp
var engine = new BattleEngine(
    formula: new StandardDamageFormula(),
    difficulty: new NormalDifficultyMode(),
    plugins: new IBattlePlugin[]
    {
        new NuzlockePlugin(),
        new TurboPlugin(),
    });
```

Multiple plugins can be active simultaneously. Events fire in registration order.

## NuzlockePlugin

Enforces Nuzlocke challenge rules:

- A fainted Pokémon is **permanently dead** — removed from the party at `OnFaint`.
- Only the **first encounter** per route is catchable (tracked in `GameState.Flags`).
- If the entire party faints, `OnBattleEnd` sets `GameState.Flags["NUZLOCKE_FAILED"]`.

```csharp
var plugin = new NuzlockePlugin();
// No configuration needed — rules are fixed by the challenge spec
```

## RandomizerPlugin

Randomizes wild encounters and trainer Pokémon at battle start:

```csharp
var plugin = new RandomizerPlugin(seed: 42);
// seed = 0 (default) → random seed from Environment.TickCount64
```

`OnTurnStart` swaps each combatant's species with a random species from the same generation.

## TurboPlugin

Speeds up battles by removing delays between actions. Intended for speedrun or test scenarios.

```csharp
var plugin = new TurboPlugin();
// Sets all animation/wait flags to instant in BattleState metadata
```

## Custom plugins

Implement `IBattlePlugin` from `SDK.Core`:

```csharp
public class LogPlugin : IBattlePlugin
{
    public string Name => "LogPlugin";

    public BattleState OnTurnStart(BattleState state)
    {
        Console.WriteLine($"Turn {state.TurnNumber}");
        return state; // always return state (modified or unchanged)
    }

    public BattleState OnDamageDealt(BattleState state, DamageResult result)
    {
        Console.WriteLine($"Dealt {result.Damage} damage (crit: {result.IsCritical})");
        return state;
    }

    public BattleState OnFaint(BattleState state, int faintedIndex) => state;
    public BattleState OnBattleEnd(BattleState state) => state;
}
```
