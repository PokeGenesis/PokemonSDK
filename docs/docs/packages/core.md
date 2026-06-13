---
sidebar_position: 2
---

# PokeForge.SDK.Core

Core domain layer entities, interfaces, and value objects. **Zero external NuGet dependencies.**

```bash
dotnet add package PokeForge.SDK.Core
```

All other SDK packages depend on `SDK.Core`. It is the only package you can safely reference from any project without a transitive dependency chain.

## Key interfaces

### IDamageFormula

Pluggable damage calculation. Implement to replace the standard Gen I–IX formula.

```csharp
public interface IDamageFormula
{
    int Calculate(DamageContext ctx);
}
```

### IDifficultyMode

Controls AI move selection and damage scaling.

```csharp
public interface IDifficultyMode
{
    MoveChoice SelectMove(BattleState state, int trainerIndex);
    float DamageMultiplier { get; }
}
```

### IBattlePlugin

Hook into battle events without forking the engine. Registered via `PluginRegistry`.

```csharp
public interface IBattlePlugin
{
    string Name { get; }
    BattleState OnTurnStart(BattleState state);
    BattleState OnDamageDealt(BattleState state, DamageResult result);
    BattleState OnFaint(BattleState state, int faintedIndex);
    BattleState OnBattleEnd(BattleState state);
}
```

## Key entities

| Type               | Description                                     |
| ------------------ | ----------------------------------------------- |
| `Species`          | Pokémon species: base stats, types, generation |
| `Move`             | Move: power, accuracy, PP, type, category      |
| `Item`             | Item: effect type, flags                       |
| `TranslationEntry` | Translation row: locale, entity key, value   |

## Value objects

| Type            | Description                                            |
| --------------- | ------------------------------------------------------ |
| `StatBlock`     | Immutable HP/Atk/Def/SpA/SpD/Spe snapshot              |
| `TypeMatchup`   | Effectiveness multiplier between two types             |
| `DamageContext` | Input to `IDamageFormula.Calculate`                    |
| `DamageResult`  | Output: final damage, crit flag, type effectiveness |
