---
sidebar_position: 1
---

# PokemonSDK

**PokemonSDK** is an open-source C# / .NET 10 SDK for building Pokémon fan-games. It gives you a plug-in-ready battle engine, a multilingual SQLite database seeded with 9 generations, a MoonSharp Lua scripting runtime, and a MonoGame HD render pipeline — so you ship a game, not a framework.

## Install

Add only the packages you need:

```bash
# Minimum — entities and interfaces
dotnet add package PokeForge.SDK.Core

# Add SQLite data layer (9 gens seeded)
dotnet add package PokeForge.SDK.Data

# Add headless battle engine
dotnet add package PokeForge.SDK.Battle

# Add Lua scripting + JSON saves
dotnet add package PokeForge.SDK.Scripting

# Add MonoGame DesktopGL runtime (cross-platform)
dotnet add package PokeForge.SDK.MonoGame

# Add developer tools (sprite validation, Fakemon pipeline)
dotnet add package PokeForge.SDK.Tools

# Add battle plugins (Nuzlocke / Randomizer / Turbo)
dotnet add package PokeForge.SDK.Plugins

# Add TTS narration (Piper TTS + Windows Speech)
dotnet add package PokeForge.SDK.Plugins.TTS
```

## 30-second example

```csharp
// Seed the database
await using var db = new PokemonDbContext(options);
await db.Database.MigrateAsync();

// Run a battle
var state = BattleState.Start(playerTeam, rivalTeam);
var engine = new BattleEngine(new StandardDamageFormula(), new NormalDifficultyMode());
state = engine.ExecuteTurn(state, playerMove, rivalMove);

// Run a Lua script
var lua = new LuaScriptEngine(gameState);
lua.Execute("game.give_badge('Boulder Badge')");
```

## Packages

| Package | Purpose |
|---------|---------|
| [SDK.Core](packages/core) | Entities, interfaces, value objects — zero NuGet dependencies |
| [SDK.Data](packages/data) | EF Core 10 + SQLite, migrations, 9-generation seed |
| [SDK.Battle](packages/battle) | Headless 1v1 battle engine with pluggable damage formulas |
| [SDK.Scripting](packages/scripting) | MoonSharp Lua sandbox, GameState, JSON save system |
| [SDK.MonoGame](packages/monogame) | MonoGame DesktopGL runtime, xBR×4 upscaling, Tiled maps |
| [SDK.Tools](packages/tools) | Sprite validator, atlas packer, Fakemon assembly pipeline |
| [SDK.Plugins](packages/plugins) | Nuzlocke, Randomizer, Turbo battle plugins |
| [SDK.Plugins.TTS](packages/plugins-tts) | INarrationPlugin, Piper TTS, Windows Speech backends |

## CLI — `pokeforge`

Scaffold, seed, and validate from the terminal:

```bash
dotnet tool install -g PokeForge.CLI
pokeforge new MyGame      # scaffold a new project from the SDK sample
pokeforge seed            # seed the SQLite database
pokeforge doctor          # check runtime dependencies
```

See [CLI reference](cli/) for all commands.

## Requirements

| Requirement | Details |
|-------------|---------|
| .NET SDK | 10.0 or later |
| SQLite | bundled via Microsoft.EntityFrameworkCore.Sqlite |
| MonoGame | SDL2 + OpenGL (Linux: `libsdl2-dev`, `libopenal-dev`) |
| TTS — Piper | `piper` binary on PATH (Linux/macOS) |
| TTS — Windows Speech | Windows only, no extra install |
