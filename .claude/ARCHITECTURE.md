# Architecture — PokemonSDK

## Arborescence complète

```
PokemonSDK/
├── src/
│   ├── SDK.Core/               ← Domaine pur — ZÉRO dépendance NuGet externe
│   │   ├── Entities/           ← PokemonSpecies, PokemonForm, Move, Ability, Item...
│   │   ├── Enums/              ← MoveType, WeatherType, DifficultyMode, TimeOfDay, MoveCategory...
│   │   ├── Interfaces/         ← IBattleEngine, IScriptEngine, ISaveSystem, IInputProvider, IBattlePlugin...
│   │   └── ValueObjects/       ← BattleConfig, WorldConfig, BattleResult, DamageResult...
│   │
│   ├── SDK.Data/               ← EF Core 10 + SQLite + migrations
│   │   ├── Context/            ← PokemonDbContext, Fluent API configs
│   │   ├── Migrations/         ← 001_InitialCreate, 002_AddBattleSchema, 003_World, 004_Scripting...
│   │   ├── Repositories/       ← Implémentations IRepository<T>
│   │   ├── Seeders/            ← DataSeeder (species/forms), BattleDataSeeder (types/moves/chart)
│   │   └── Extensions/         ← DbContextExtensions.GetByGeneration(), GetTranslations()
│   │
│   ├── SDK.Battle/             ← Moteur de combat headless — ZÉRO MonoGame, ZÉRO Scripting
│   │   ├── Engine/             ← BattleEngine, BattleState (record immuable), BattlePokemon, BattleAction
│   │   ├── Formulas/           ← IDamageFormula, Gen13DamageFormula, Gen4PlusDamageFormula, DamageFormulaFactory
│   │   ├── AI/                 ← IDifficultyMode, StoryModeAI, HardModeAI, DifficultyModeFactory
│   │   ├── Plugins/            ← IBattlePlugin, IBattleEventHook, PluginRegistry
│   │   └── Effects/            ← StatusEffectSystem, WeatherEffects
│   │
│   ├── SDK.Plugins/            ← Plugins officiels — chacun = projet NuGet indépendant
│   │   ├── SDK.Plugins.Nuzlocke/    ← NuzlockePlugin : mort permanente, une capture par zone
│   │   ├── SDK.Plugins.Randomizer/  ← RandomizerPlugin : seed reproductible, randomisation complète
│   │   └── SDK.Plugins.Turbo/       ← TurboPlugin : accélération animations et transitions
│   │
│   ├── SDK.Scripting/          ← MoonSharp sandbox — ZÉRO MonoGame
│   │   ├── Engine/             ← IScriptEngine, LuaScriptEngine (Preset_SoftSandbox)
│   │   ├── Api/                ← Bindings C#↔Lua : dialog, battle, flags, items, player
│   │   └── GameState/          ← GameState, ISaveSystem, SaveSystem (System.Text.Json)
│   │
│   ├── SDK.Tools/              ← Outils DX — ZÉRO MonoGame (CI headless)
│   │   ├── AssetPipeline/
│   │   │   ├── SpriteScanner.cs      ← Scan dossier PNG récursif
│   │   │   ├── SpriteValidator.cs    ← Valide tailles (48×48/96×96/16×16/128×128), nommage, alpha
│   │   │   ├── AtlasPacker.cs        ← Génère atlas + MGCB content config auto
│   │   │   └── SqliteSyncer.cs       ← Mappe asset_key → pokemon_forms.asset_key en SQLite
│   │   └── LuaDevTools/
│   │       ├── LuaHotReloader.cs     ← FileSystemWatcher + LuaScriptEngine.Reload()
│   │       ├── LuaConsole.cs         ← REPL ingame, toggle ~, historique, autocomplete
│   │       └── LuaErrorOverlay.cs    ← Overlay fichier + ligne + message (#if DEBUG)
│   │
│   ├── SDK.Cli/                ← CLI `pokeforge` — dotnet tool global (Phase 10)
│   │   ├── Commands/
│   │   │   ├── NewCommand.cs         ← scaffold projet depuis template embarqué
│   │   │   ├── AssetSyncCommand.cs   ← wrapper SDK.Tools.AssetPipeline
│   │   │   ├── SeedCommand.cs        ← wrapper PokeAPI seeder
│   │   │   ├── BuildCommand.cs       ← dotnet publish multi-plateforme
│   │   │   └── DoctorCommand.cs      ← vérification environnement
│   │   └── Templates/
│   │       └── StarterGame/          ← copie du sample stabilisé (Phase 9)
│   │
│   └── SDK.MonoGame/           ← Runtime jouable — entry point + composition root
│       ├── Game1.cs             ← Entry point MonoGame + composition root DI
│       ├── World/               ← WorldSystem, PlayerSystem, TransitionSystem
│       ├── Rendering/
│       │   ├── RenderPipeline.cs     ← RenderTarget2D 480×270 → xBR ×4 → 1920×1080
│       │   ├── SpriteRenderer.cs     ← SpriteBatch wrapper avec shader pipeline
│       │   ├── Camera2D.cs           ← Smooth follow, bounds, pixel-perfect
│       │   ├── TilemapRenderer.cs    ← Chunk-based 16×16, layers ordonnés
│       │   └── Shaders/
│       │       ├── xBR.fx            ← Upscaling intelligent (HLSL→GLSL auto)
│       │       ├── Bloom.fx          ← Effets lumineux combat
│       │       ├── DayNight.fx       ← Tint + saturation selon TimeOfDay
│       │       └── PaletteSwap.fx    ← Swap couleur par couche (shinies, Fakemons)
│       ├── Input/               ← KeyboardInputProvider, GamepadInputProvider
│       ├── UI/                  ← DialogueBox, BattleUI, HUD, PokegearUI
│       └── Audio/               ← AudioManager (BGM loop seamless + SFX pooled)
│
├── samples/
│   └── StarterGame/            ← Jeu minimal de référence (Phase 9) — NuGet only
│
├── tests/
│   ├── SDK.Core.Tests/
│   ├── SDK.Data.Tests/
│   ├── SDK.Battle.Tests/
│   └── SDK.Scripting.Tests/
│
├── docs/                       ← Site Docusaurus (Phase 11) — GitHub Pages
│
├── data/
│   ├── PokemonSDK.db           ← SQLite (.gitignore en prod)
│   ├── maps/                   ← Fichiers .tmx Tiled
│   └── scripts/                ← Scripts Lua (events, dialogues, quêtes)
│
├── .claude/                    ← Fichiers contexte détaillés pour Claude Code
│   ├── ARCHITECTURE.md         ← CE FICHIER
│   ├── CONVENTIONS.md
│   ├── RENDERING.md
│   ├── PLUGINS.md
│   ├── DX.md
│   └── CICD.md
│
├── .github/workflows/          ← CI/CD GitHub Actions
├── CLAUDE.md                   ← Index principal (chargé automatiquement)
├── REQUIREMENTS.md
├── ROADMAP.md
├── PROJECT.md
└── STATE.md
```

---

## Règles de dépendances (NON NÉGOCIABLES)

```
SDK.Core               ← ZÉRO dépendance NuGet externe
SDK.Data               ← SDK.Core + EF Core 10
SDK.Battle             ← SDK.Core uniquement
SDK.Scripting          ← SDK.Core + MoonSharp
SDK.Plugins.Nuzlocke   ← SDK.Core + SDK.Battle uniquement
SDK.Plugins.Randomizer ← SDK.Core + SDK.Battle uniquement
SDK.Plugins.Turbo      ← SDK.Core + SDK.Battle uniquement
SDK.Tools              ← SDK.Core + SDK.Data + SDK.Scripting (jamais MonoGame — CI headless)
SDK.Cli                ← SDK.Tools + System.CommandLine (jamais MonoGame)
SDK.MonoGame           ← SDK.Core + MonoGame + SDK.Battle + SDK.Scripting (via Func factory)
samples/StarterGame    ← PokéForge.SDK via NuGet uniquement (jamais référence projet)
```

**SDK.MonoGame ne référence pas SDK.Scripting directement.**
`IScriptEngine` est injecté via `Func<IScriptEngine>` dans `Game1` (composition root).

**Les plugins ne référencent jamais SDK.MonoGame.**
Ils reçoivent les hooks via `IBattlePlugin` — jamais accès direct au renderer.

---

## Schéma SQLite — Tables clés

```sql
-- snake_case obligatoire sur toutes les tables et colonnes

CREATE TABLE pokemon_species (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    identifier      TEXT    NOT NULL UNIQUE,   -- "bulbasaur", "pikachu"
    generation      INTEGER NOT NULL,           -- 1..9 (NOT NULL obligatoire partout)
    origin_region   TEXT    NOT NULL,           -- "kanto", "alola"...
    type1_id        INTEGER NOT NULL REFERENCES types(id),
    type2_id        INTEGER     NULL REFERENCES types(id)
);

-- Formes indépendantes — jamais colonnes nullables sur pokemon_species
CREATE TABLE pokemon_forms (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    species_id  INTEGER NOT NULL REFERENCES pokemon_species(id),
    form_key    TEXT        NULL,               -- NULL = forme de base, "alola", "mega_x"...
    asset_key   TEXT    NOT NULL,               -- "00025" ou "00025_alola"
    is_default  INTEGER NOT NULL DEFAULT 1,
    generation  INTEGER NOT NULL
);

CREATE TABLE pokemon_base_stats (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    form_id         INTEGER NOT NULL REFERENCES pokemon_forms(id),
    hp              INTEGER NOT NULL,
    attack          INTEGER NOT NULL,
    defense         INTEGER NOT NULL,
    special_attack  INTEGER NOT NULL,
    special_defense INTEGER NOT NULL,
    speed           INTEGER NOT NULL
);

-- Jamais name_fr / name_en sur les entités — tout passe par ici
CREATE TABLE translations (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    entity_type TEXT    NOT NULL,  -- "species", "move", "ability", "item"
    entity_id   INTEGER NOT NULL,
    locale      TEXT    NOT NULL,  -- "fr", "en", "es", "de", "it"
    field       TEXT    NOT NULL,  -- "name", "description", "flavor_text"
    value       TEXT    NOT NULL,
    UNIQUE(entity_type, entity_id, locale, field)
);

CREATE TABLE types (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    identifier  TEXT    NOT NULL UNIQUE,
    generation  INTEGER NOT NULL
);

CREATE TABLE type_effectiveness (
    attacker_type_id INTEGER NOT NULL REFERENCES types(id),
    defender_type_id INTEGER NOT NULL REFERENCES types(id),
    damage_factor    REAL    NOT NULL,  -- 0.0 / 0.5 / 1.0 / 2.0
    generation       INTEGER NOT NULL,
    PRIMARY KEY (attacker_type_id, defender_type_id, generation)
);

-- Tables à ajouter en phases suivantes :
-- moves, abilities, learnsets, items, encounter_zones,
-- encounter_entries, trainers, trainer_pokemon, badges
```

---

## Migration .NET 10 — Checklist première session

```bash
# 1. Vérifier .NET 10 installé
dotnet --version   # doit afficher 10.x.x

# 2. Vérifier compatibilité packages clés avant de créer la solution
# MonoGame.Framework.DesktopGL — vérifier support net10.0 sur NuGet
# MoonSharp 2.0.0 — vérifier support net10.0
# Microsoft.EntityFrameworkCore.Sqlite — utiliser version 10.0.x

# 3. Créer la solution
dotnet new sln -n PokemonSDK -o .
dotnet new classlib -n SDK.Core    -f net10.0 -o src/SDK.Core
dotnet new classlib -n SDK.Data    -f net10.0 -o src/SDK.Data
dotnet new classlib -n SDK.Battle  -f net10.0 -o src/SDK.Battle
dotnet new classlib -n SDK.Scripting -f net10.0 -o src/SDK.Scripting
dotnet new mgdesktopgl -n SDK.MonoGame -f net10.0 -o src/SDK.MonoGame
dotnet sln add src/SDK.Core src/SDK.Data src/SDK.Battle src/SDK.Scripting src/SDK.MonoGame

# 4. Vérifier SDK.Core zéro dépendances
dotnet list src/SDK.Core/SDK.Core.csproj package  # doit être vide
```
