---
sidebar_position: 4
---

# Step 3: Lua Script and Badge

## Create the Script

Create `scripts/gym_first.lua` in your project:

```lua
-- Award the first gym badge and record progress
game.give_badge("Badge Herbier")
game.set_flag("GYM1_DONE", true)
game.save()
```

The `game` global is exposed by `LuaScriptEngine`. It runs inside `Preset_SoftSandbox`: no filesystem access, no network, no OS calls.

## Wire It in Game1.cs

Add the following code after the battle block from Step 2:

```csharp
using PokeForge.SDK.Scripting;

// Create engine with current GameState and SaveSystem
var scriptEngine = new LuaScriptEngine(gameState, saveSystem);

// Execute the gym script
scriptEngine.ExecuteFile("scripts/gym_first.lua");

// Read back the flag to confirm it persisted
bool gymCleared = gameState.Flags.TryGetValue("GYM1_DONE", out var v)
    && v.GetBoolean();

Console.WriteLine($"GYM1_DONE:  {gymCleared}");
Console.WriteLine($"Badges:     {string.Join(", ", gameState.Badges)}");
```

`gameState` and `saveSystem` come from DI. The scaffolded `Program.cs` already registers both.

## Run It

```bash
dotnet run -- --headless
```

Expected output:

```
Winner: Bulbasaur
Turns:  7
GYM1_DONE:  True
Badges:     Badge Herbier
```

Check that the save file was created:

```bash
ls saves/
# default.json
```

Open `saves/default.json`:

```json
{
  "Flags": { "GYM1_DONE": true },
  "Badges": ["Badge Herbier"],
  "Money": 0
}
```

:::note
`JsonSaveSystem` writes to `saves/{slot}.json` inside the application data folder. The default slot is `"default"`.
:::

## What's Next

You have a working fan game: scaffold, battle, Lua badge, and save.

From here:

| Topic | Guide |
|-------|-------|
| Battle system in depth | [Battle package](../packages/battle) |
| Lua scripting patterns | [Scripting package](../packages/scripting) |
| Plugins (Nuzlocke, Randomizer) | [Plugins](../packages/plugins) |
| CLI commands reference | [CLI pokeforge](../cli) |
