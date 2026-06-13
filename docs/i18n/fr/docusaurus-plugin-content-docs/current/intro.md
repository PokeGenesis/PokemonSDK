---
sidebar_position: 1
---

# PokemonSDK

**PokemonSDK** est un SDK C# / .NET 10 open-source pour créer des fan-games Pokémon. Il vous fournit un moteur de combat, une base SQLite multilingue pré-remplie sur 9 générations, un runtime Lua, et un pipeline de rendu HD MonoGame pour que vous vous concentriez sur votre jeu, pas sur l'infrastructure.

## Installation

Ajoutez uniquement les packages dont vous avez besoin :

```bash
# Entités et interfaces de base (requis par tous les autres packages)
dotnet add package PokeForge.SDK.Core

# Couche de données: EF Core 10 + SQLite, 9 générations
dotnet add package PokeForge.SDK.Data

# Moteur de combat headless
dotnet add package PokeForge.SDK.Battle

# Scripting Lua MoonSharp + sauvegardes JSON
dotnet add package PokeForge.SDK.Scripting

# Runtime MonoGame DesktopGL (cross-platform)
dotnet add package PokeForge.SDK.MonoGame

# Outils développeur: validation sprites, pipeline Fakemon
dotnet add package PokeForge.SDK.Tools

# Plugins de combat: Nuzlocke / Randomizer / Turbo
dotnet add package PokeForge.SDK.Plugins

# Narration TTS: Piper TTS + Windows Speech
dotnet add package PokeForge.SDK.Plugins.TTS
```

## Exemple en 30 secondes

```csharp
// Préparer la base de données
await using var db = new PokemonDbContext(options);
await db.Database.MigrateAsync();

// Lancer un combat
var state = BattleState.Start(equipeJoueur, equipeRival);
var engine = new BattleEngine(new StandardDamageFormula(), new NormalDifficultyMode());
state = engine.ExecuteTurn(state, move);

// Exécuter un script Lua
var lua = new LuaScriptEngine(gameState);
lua.Execute("game.give_badge('Badge Rocher')");
```

## Packages

| Package                                 | Rôle                                                       |
| --------------------------------------- | ---------------------------------------------------------- |
| [SDK.Core](packages/core)               | Entités, interfaces, value objects (zéro dépendance NuGet) |
| [SDK.Data](packages/data)               | EF Core 10 + SQLite, migrations, données 9 générations     |
| [SDK.Battle](packages/battle)           | Moteur de combat 1v1 headless, formules pluggables         |
| [SDK.Scripting](packages/scripting)     | Sandbox Lua MoonSharp, GameState, sauvegardes JSON         |
| [SDK.MonoGame](packages/monogame)       | Runtime MonoGame DesktopGL, upscaling xBR×4, cartes Tiled  |
| [SDK.Tools](packages/tools)             | Validation sprites, packing atlas, pipeline Fakemon        |
| [SDK.Plugins](packages/plugins)         | Plugins Nuzlocke, Randomizer, Turbo                        |
| [SDK.Plugins.TTS](packages/plugins-tts) | INarrationPlugin, Piper TTS, Windows Speech                |

## CLI: `pokeforge`

Scaffoldez, remplissez et validez depuis le terminal :

```bash
dotnet tool install -g PokeForge.CLI
pokeforge new MonJeu      # crée un nouveau projet depuis le template
pokeforge seed            # remplit la base SQLite
pokeforge doctor          # vérifie les dépendances runtime
```

Voir la [référence CLI](cli/).

## Prérequis

| Prérequis            | Détails                                                |
| -------------------- | ------------------------------------------------------ |
| .NET SDK             | 10.0 ou supérieur                                      |
| SQLite               | fourni via Microsoft.EntityFrameworkCore.Sqlite        |
| MonoGame             | SDL2 + OpenGL (Linux : `libsdl2-dev`, `libopenal-dev`) |
| Piper TTS            | binaire `piper` dans le PATH (Linux/macOS)             |
| Windows Speech TTS   | Windows uniquement, aucune installation requise        |
