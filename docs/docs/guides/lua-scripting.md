---
sidebar_position: 3
---

# Lua Scripting: NPC Events and Badges

This guide shows how to create the script engine, write Lua scripts for NPC events and badges, and persist game state.

## Install

```bash
dotnet add package PokeForge.SDK.Scripting
```

## Create the Engine

`LuaScriptEngine` takes a `GameState` and a `SaveSystem`. Inject both from your composition root:

```csharp
using PokeForge.SDK.Scripting;

var scriptEngine = new LuaScriptEngine(gameState, saveSystem);
```

## Write a Script

Scripts run in a `Preset_SoftSandbox`: standard Lua libraries are available, but `os`, `io`, and network access are blocked. Scripts interact with the game through the `game` and `player` globals:

```lua
-- award_badge.lua
-- Called when the player defeats the gym leader

if not game.get_flag("gym1_beaten") then
  game.set_flag("gym1_beaten", true)
  player.give_badge(1)
  player.heal_party()
  game.show_dialog("Congratulations! You earned the Stone Badge.")
  game.save()
end
```

Run a script file:

```csharp
await scriptEngine.RunFileAsync("scripts/award_badge.lua");
```

## Sandbox Constraints

The MoonSharp `Preset_SoftSandbox` enforces these limits:

| Blocked | Allowed |
|---------|---------|
| `os.*` | `math.*`, `string.*`, `table.*` |
| `io.*` | `game.*`, `player.*`, `sdk.*` |
| Network | `coroutine.*` |

Attempting to call a blocked global raises a `ScriptRuntimeException` that the engine catches and logs.

## GameState Flags

Flags are typed `JsonElement` values stored in `GameState.Flags`. The Lua API wraps them:

```lua
-- Set a boolean flag
game.set_flag("npc_talked", true)

-- Set a numeric flag
game.set_flag("coins_collected", 42)

-- Read a flag (returns nil if not set)
local talked = game.get_flag("npc_talked")
if talked then
  game.show_dialog("We've already spoken.")
end
```

From C# you can read flags directly:

```csharp
if (gameState.Flags.TryGetValue("gym1_beaten", out var val)
    && val.GetBoolean())
{
    // gate is unlocked
}
```

## Hot Reload in Debug Builds

In debug mode, `LuaHotReloader` watches script files and re-runs them on save:

```csharp
#if DEBUG
var hotReloader = new LuaHotReloader(scriptEngine, "scripts/");
hotReloader.Start();  // re-runs changed scripts in <500ms
#endif
```

Hot reload applies only to scripts that are safe to re-run (no side effects on re-entry). Badge-award scripts should guard with a flag check, as shown above.
