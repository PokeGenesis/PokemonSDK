# PokemonSDK

SDK open-source C# / .NET 10 pour fan-games Pokémon.
Moteur de données SQLite (9 générations), battle engine headless,
runtime MonoGame DesktopGL, système de plugins modulaire.

## Packages

| Package | Rôle |
|---------|------|
| `PokeForge.SDK.Core` | Modèles domaine, interfaces, value objects — zéro dépendance NuGet |
| `PokeForge.SDK.Data` | EF Core 10 + SQLite, schéma 9 générations, table `translations` centrale |
| `PokeForge.SDK.Battle` | Battle engine 1v1 headless, BattleState immuable, formules par génération |
| `PokeForge.SDK.Scripting` | MoonSharp Preset_SoftSandbox, GameState, SaveSystem JSON |
| `PokeForge.SDK.Plugins.Nuzlocke` | Plugin IBattlePlugin — Nuzlocke (permadeath, catch-first-only) |
| `PokeForge.SDK.Plugins.Randomizer` | Plugin IBattlePlugin — Randomizer (seed-déterministe) |
| `PokeForge.SDK.Plugins.Turbo` | Plugin IBattlePlugin — Turbo (TextSpeedMultiplier) |
| `PokeForge.SDK.Plugins.TTS` | Narration vocale — PiperTTS (cross-platform) + Windows Speech, asynchrone, non-bloquant |

## Prérequis

- .NET 10 SDK (`net10.0`)
- SQLite (fourni via `Microsoft.EntityFrameworkCore.Sqlite`)

## Installation

\`\`\`bash
dotnet add package PokeForge.SDK.Core
dotnet add package PokeForge.SDK.Data
dotnet add package PokeForge.SDK.Battle
dotnet add package PokeForge.SDK.Scripting
\`\`\`

Plugins optionnels :

\`\`\`bash
dotnet add package PokeForge.SDK.Plugins.Nuzlocke
dotnet add package PokeForge.SDK.Plugins.Randomizer
dotnet add package PokeForge.SDK.Plugins.Turbo
dotnet add package PokeForge.SDK.Plugins.TTS
\`\`\`

## DataPack

Le DataPack fournit les données de jeu prêtes à l'emploi (Pokémon, moves, types, items) pour 9 générations.

\`\`\`bash
git clone https://github.com/PokeGenesis/PokemonSDK-DataPack.git
pokeforge datapack --use ./PokemonSDK-DataPack
\`\`\`

Le DataPack est optionnel : sans lui, la base SQLite reste vide et les seeds doivent être fournis manuellement.

## Licence

[MIT](LICENSE) © 2026 PokeGenesis

---

# PokemonSDK (English)

Open-source C# / .NET 10 SDK for Pokémon fan-games.
SQLite data engine (9 generations), headless battle engine,
MonoGame DesktopGL runtime, modular plugin system.

## Packages

| Package | Role |
|---------|------|
| `PokeForge.SDK.Core` | Domain models, interfaces, value objects — zero NuGet dependency |
| `PokeForge.SDK.Data` | EF Core 10 + SQLite, 9-generation schema, central `translations` table |
| `PokeForge.SDK.Battle` | Headless 1v1 battle engine, immutable BattleState, per-generation formulas |
| `PokeForge.SDK.Scripting` | MoonSharp Preset_SoftSandbox, GameState, JSON SaveSystem |
| `PokeForge.SDK.Plugins.Nuzlocke` | IBattlePlugin — Nuzlocke (permadeath, catch-first-only) |
| `PokeForge.SDK.Plugins.Randomizer` | IBattlePlugin — Randomizer (seed-deterministic) |
| `PokeForge.SDK.Plugins.Turbo` | IBattlePlugin — Turbo (TextSpeedMultiplier) |
| `PokeForge.SDK.Plugins.TTS` | Voice narration — PiperTTS (cross-platform) + Windows Speech, async, non-blocking |

## Requirements

- .NET 10 SDK (`net10.0`)
- SQLite (provided via `Microsoft.EntityFrameworkCore.Sqlite`)

## Installation

\`\`\`bash
dotnet add package PokeForge.SDK.Core
dotnet add package PokeForge.SDK.Data
dotnet add package PokeForge.SDK.Battle
dotnet add package PokeForge.SDK.Scripting
\`\`\`

Optional plugins:

\`\`\`bash
dotnet add package PokeForge.SDK.Plugins.Nuzlocke
dotnet add package PokeForge.SDK.Plugins.Randomizer
dotnet add package PokeForge.SDK.Plugins.Turbo
dotnet add package PokeForge.SDK.Plugins.TTS
\`\`\`

## DataPack

The DataPack provides ready-to-use game data (Pokémon, moves, types, items) for 9 generations.

\`\`\`bash
git clone https://github.com/PokeGenesis/PokemonSDK-DataPack.git
pokeforge datapack --use ./PokemonSDK-DataPack
\`\`\`

The DataPack is optional: without it, the SQLite database remains empty and seeds must be provided manually.

## License

[MIT](LICENSE) © 2026 PokeGenesis
