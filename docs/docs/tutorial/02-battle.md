---
sidebar_position: 3
---

# Step 2: First Battle

Open `src/Game1.cs` and locate the `Initialize()` method (or the startup block called from `LoadContent`).

Add the following code:

```csharp
using Microsoft.Extensions.DependencyInjection;
using PokeForge.SDK.Battle;
using PokeForge.SDK.Battle.Formulas;
using PokeForge.SDK.Battle.Difficulty;
using PokeForge.SDK.Data;

// Resolve DbContext from DI
var db = Services.GetRequiredService<PokemonDbContext>();

// Load two Pokémon by Dex ID
var bulbasaur  = await db.PokemonSpecies.FindAsync(1)
    ?? throw new Exception("Run 'pokeforge seed' first.");
var charmander = await db.PokemonSpecies.FindAsync(4)
    ?? throw new Exception("Run 'pokeforge seed' first.");

// Build combatants
var player  = BattlePokemon.FromSpecies(bulbasaur,  level: 5);
var opponent = BattlePokemon.FromSpecies(charmander, level: 5);

// Configure battle
var config  = new BattleConfig { MaxTurns = 200 };
var formula = new StandardDamageFormula();
var mode    = new NormalDifficultyMode();
var engine  = new BattleEngine(player, opponent, formula, mode, config);

// Run turns until battle ends
while (!engine.State.IsOver)
{
    var move = engine.State.PlayerPokemon.Moves[0]; // always use first move
    engine.ExecuteTurn(move);
}

var result = engine.State.Result!;
Console.WriteLine($"Winner: {(result.PlayerWon ? player.Name : opponent.Name)}");
Console.WriteLine($"Turns:  {result.TurnsElapsed}");
```

## Run It

```bash
dotnet run -- --headless
```

Expected console output:

```
Winner: Bulbasaur
Turns:  7
```

The exact winner and turn count depend on level and RNG. The battle always terminates.

## Understanding the Code

**BattleState is immutable.** Every call to `ExecuteTurn` returns a new `BattleState` via C# `record` + `with`. The previous state is never mutated, so replays and undo are free.

**IDamageFormula is pluggable.** Swap `StandardDamageFormula` for `Gen1DamageFormula` to match Generation 1 integer-truncation behavior, or implement your own.

**BattleConfig controls limits.** `MaxTurns = 200` prevents infinite loops when two Pokémon keep missing. `Result.EndReason` is `"MaxTurns"` if the timeout fires.

:::note
See the [Battle package reference](../packages/battle) for all BattleEngine options, move selection strategies, and plugin hooks.
:::

Next: [Step 3: Lua Script and Badge](./lua-badge)
