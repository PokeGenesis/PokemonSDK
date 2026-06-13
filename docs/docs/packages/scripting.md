---
sidebar_position: 5
---

# PokeForge.SDK.Scripting

MoonSharp Lua sandbox, persistent GameState with typed flags, and a JSON save system.

```bash
dotnet add package PokeForge.SDK.Scripting
```

## Lua sandbox

Scripts run inside MoonSharp `Preset_SoftSandbox` with no filesystem access, no network, no OS calls. Standard Lua library is available. C# interop is exposed via whitelisted globals (`game`, `player`).

```lua
-- Give a badge and save (scripts/boulder_badge.lua)
game.give_badge("Boulder Badge")
game.set_flag("GYM1_CLEARED", true)
player.heal_party()
game.save()
```

## LuaScriptEngine

```csharp
var engine = new LuaScriptEngine(gameState, saveSystem);

// Execute a Lua string directly
engine.Execute("game.give_badge('Boulder Badge')");

// Load a .lua file from disk
engine.ExecuteFile("scripts/intro.lua");
```

## GameState

`GameState` holds all persistent data exposed to Lua scripts:

```csharp
public class GameState
{
    public Dictionary<string, JsonElement> Flags { get; init; } = new();
    public List<string> Badges { get; init; } = new();
    public int Money { get; set; }
    public MapPosition PlayerPosition { get; set; }
    // ...
}
```

Flags use `System.Text.Json.JsonElement` so any JSON-serializable value works:

```csharp
// Set a flag from C#
gameState.Flags["RIVAL_NAME"] = JsonSerializer.SerializeToElement("Blue");

// Read a boolean flag
bool done = gameState.Flags.TryGetValue("INTRO_DONE", out var v)
    && v.GetBoolean();
```

## ISaveSystem

```csharp
public interface ISaveSystem
{
    Task SaveAsync(GameState state, string slot = "default");
    Task<GameState?> LoadAsync(string slot = "default");
}
```

The bundled `JsonSaveSystem` writes `saves/{slot}.json` to the OS application-data folder.

```csharp
var saveSystem = new JsonSaveSystem(basePath: "saves/");
await saveSystem.SaveAsync(gameState, "slot1");
var loaded = await saveSystem.LoadAsync("slot1");
```

## Hot reload

In non-production builds, `LuaScriptEngine` watches `.lua` files with a `FileSystemWatcher`. Saving a script file re-executes it automatically (no restart needed during development).
