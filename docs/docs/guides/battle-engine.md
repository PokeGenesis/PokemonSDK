---
sidebar_position: 2
---

# BattleEngine: Wire 1v1 Combat

This guide shows how to configure formulas and difficulty, run a battle loop, and integrate the battle engine with MonoGame.

## Install

```bash
dotnet add package PokeForge.SDK.Battle
```

## Configure Formulas and Difficulty

`BattleConfig` accepts any `IDamageFormula` and `IDifficultyMode`. Two implementations ship with the SDK:

| Type | Options |
|------|---------|
| `IDamageFormula` | `StandardDamageFormula`, `Gen1DamageFormula` |
| `IDifficultyMode` | `NormalDifficultyMode`, `HardDifficultyMode` |

```csharp
using PokeForge.SDK.Battle;

var config = new BattleConfig(
    formula: new StandardDamageFormula(),
    difficulty: new NormalDifficultyMode()
);

var engine = new BattleEngine(config);
```

Switch to `Gen1DamageFormula` to reproduce Generation 1 damage calculations, including the critical-hit formula.

## Run a Battle Loop

`BattleState` is an immutable record: every `ExecuteTurn` call returns a new state via `with`-expressions. Never mutate the state object directly.

```csharp
var state = BattleState.Start(playerPokemon, opponentPokemon, config);

while (!state.IsOver)
{
    var moveIndex = ChooseMove(state);            // your UI picks a move
    state = engine.ExecuteTurn(state, moveIndex);
    RenderBattleFrame(state);                    // your render pass
}

Console.WriteLine($"Winner: {state.Winner?.Name ?? "Draw"}");
```

`BattleState.IsOver` becomes `true` when one side has no remaining HP or all Pokémon have fainted.

Note on status effects: Sleep and Freeze do not auto-skip the affected turn. The engine waits for the player to input a move before resolving the status, matching accurate game behavior.

## Read Battle Events

`state.LastEvent` carries structured data about what happened on the previous turn:

```csharp
switch (state.LastEvent)
{
    case MoveEvent mv:
        Console.WriteLine($"{mv.User.Name} used {mv.MoveName}");
        break;
    case DamageEvent dmg:
        Console.WriteLine($"{dmg.Target.Name} lost {dmg.Amount} HP");
        break;
    case StatusEvent st:
        Console.WriteLine($"{st.Pokemon.Name} is now {st.Status}");
        break;
}
```

## Integrate with MonoGame

Call `ExecuteTurn` inside `Update()` after input is resolved, then read state in `Draw()`:

```csharp
// Game1.Update()
if (_inputReady && !_battleState.IsOver)
{
    _battleState = _engine.ExecuteTurn(_battleState, _selectedMove);
    _inputReady = false;
}

// Game1.Draw()
_battleRenderer.Draw(_battleState, spriteBatch);
```

Keep `ExecuteTurn` out of `Draw()`. The battle state update is logic, not rendering.
