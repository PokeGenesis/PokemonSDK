# PokemonSDK

## What This Is

SDK open-source C# / .NET 10 pour fan-games Pokémon. Moteur de données SQLite (9 générations), battle engine headless, runtime MonoGame DesktopGL, système de plugins modulaire. Repo éditeur séparé : PokeForge-Editor (Avalonia).

## Core Value

Un développeur peut brancher ce SDK et obtenir immédiatement un moteur de combat, une base de données Pokémon multilingue et un système de quêtes fonctionnel — sans réimplémenter les règles de base.

## Current State

| Attribute | Value |
|-----------|-------|
| Type | Application |
| Version | 0.1.0 (packages publiables) |
| Status | Phase 8 complete — Phase 9 (Sample Project) next (v1.0) |
| Last Updated | 2026-06-07 — Phase 8 NuGet Distribution shipped |

## Requirements

→ Source complète : `REQUIREMENTS.md` (34 requirements, traceability complète)

### Core Features

- SDK.Core : domaine pur, entités, interfaces, value objects — zéro NuGet externe
- SDK.Data : EF Core 10 + SQLite, schéma 9 générations, table translations centrale
- SDK.Battle : moteur 1v1 headless, BattleState immuable, formules par génération, IA configurable
- SDK.Scripting : MoonSharp Preset_SoftSandbox, GameState, SaveSystem JSON
- SDK.MonoGame : runtime jouable, RenderPipeline HD (480×270 → xBR ×4 → 1920×1080)

### Shipped in Phase 1 ✅
- DATA-01→06 — Solution scaffold, SDK.Core entités, EF Core 10 + SQLite, migrations, filtres génération, table translations centrale — *2026-06-02*
- PLAT-01 — Tous .csproj net10.0 (vérifié automatiquement par PlatformTests) — *2026-06-02*
- PLAT-03 — Zéro chemin Windows hardcodé dans src/ (vérifié automatiquement) — *2026-06-02*

### Shipped in Phase 2 ✅
- BATTLE-01 — Combat 1v1 headless de start à KO — *2026-06-04*
- BATTLE-02 — IDamageFormula × 2 (Gen1DamageFormula, StandardDamageFormula) + ITypeChart + TypeChart — *2026-06-04*
- BATTLE-03 — IDifficultyMode × 2 (StoryDifficultyMode, HardDifficultyMode) — *2026-06-04*
- BATTLE-07 — BattleConfig (génération, crit, difficulté) — *2026-06-04*

### Shipped in Phase 3 ✅
- MAP-01 — RenderPipeline HD 480×270 → xBR ×4 → 1920×1080 (D-14, D-15) — *2026-06-05*
- MAP-03 — Jour/nuit — DayNight tint via RenderPipeline 3-pass — *2026-06-05*
- PLAT-02 — CI Windows + Linux — matrix ubuntu-latest + windows-latest activé — *2026-06-05*

### Shipped in Phase 4 ✅
- SCRIPT-01 — SDK.Scripting: IScriptEngine + LuaScriptEngine SoftSandbox + GameState + coroutines — *2026-06-05*
- SCRIPT-02 — Migration 004 (trainers/badges) + Lua badge/flag API + OnNpcInteraction — *2026-06-05*
- SCRIPT-03 — ISaveSystem + SaveSystem JSON + DialogueBox + scripts prod + Game1 wiring — *2026-06-05*

### Shipped in Phase 5 ✅
- BATTLE-04 — NuzlockePlugin (IBattlePlugin, permadeath, catch-first-only) — *2026-06-06*
- BATTLE-05 — RandomizerPlugin (species randomization seed-déterministe) — *2026-06-06*
- BATTLE-06 — TurboPlugin (IsActive, TextSpeedMultiplier float.MaxValue) — *2026-06-06*
- CHAR-01 — Character + VillainGroup + VillainMember entities, Migration AddCharacterData, CharacterDataSeeder D-22 — *2026-06-06*

### Validated (À implémenter .NET 10)
- MAP-02 — Tilemap overworld (TilemapRenderer stub — Phase 7 DX)

### Shipped in Phase 7 ✅
- DX-01 — SpriteValidator + AtlasPacker + SqliteSyncer + CLI asset-validate/asset-sync — *2026-06-06*
- DX-02 — LuaHotReloader (#if DEBUG) + LuaErrorOverlay + LuaConsole REPL (toggle ~) + MGCB DefaultFont.xnb — *2026-06-06*

### Shipped in Phase 8 ✅
- DX-03 — 7 packages PokeForge.SDK.* v0.1.0 publiables NuGet.org — métadonnées, licences, publish-nuget.yml CI/CD, smoke test consumer D-19 — *2026-06-07*

### Shipped in Phase 9 ✅
- DX-04 — StarterGame : BattleEngine 1v1 + NuzlockePlugin + Lua badge + ISaveSystem F5/F9 — demo jouable NuGet-only (D-19) — *2026-06-07*

### Active (v2.0)
- Phase 10 : CLI pokeforge (scaffold sample depuis template embarqué — D-20)
- Phase 11 : Documentation (APIs stables uniquement — D-21)

### Out of Scope
- Connexion online / trades réseau
- Support mobile (iOS/Android)
- Moteur 3D / fenêtre WebView Electron
- Données officielles Nintendo/Game Freak

## Constraints

### Technical
- .NET 10 (net10.0 dans tous les .csproj) — D-01
- MonoGame.Framework.DesktopGL — jamais WindowsDX — D-02
- EF Core 10 + migrations — D-03
- MoonSharp Preset_SoftSandbox — jamais NLua — D-04
- System.Text.Json — jamais Newtonsoft.Json — D-10

### Business
- Résolution 480×270 → ×4 → 1920×1080 figée — D-14
- Nuzlocke/Randomizer/Turbo = plugins, jamais modes hardcodés — D-13
- SDK.MonoGame ne référence pas SDK.Scripting directement — D-06

## Key Decisions

→ Source complète : `PROJECT.md` (D-01 à D-21 avec rationales)

## Success Metrics

| Metric | Target | Current | Status |
|--------|--------|---------|--------|
| Phase 1 complete | dotnet ef + requêtes génération/translations OK | 12/12 tests verts | ✅ Complete |
| Phase 2 complete | Combat 1v1 headless de start à KO | 47/47 tests verts | ✅ Complete |
| Phase 3 complete | Joueur sur tilemap, rencontre, CI matrix | 97/97 tests verts | ✅ Complete |
| Phase 4 complete | Lua sandbox, badges, save/load | 97/97 tests verts | ✅ Complete |
| v0.1 | Joueur sur map, combat, badge, save/load | 97 tests + Phase 5 done | ✅ Complete |
| Phase 5 complete | 3 plugins + 3 entités Character, D-22 | 126/126 tests verts | ✅ Complete |
| Phase 7 complete | Asset pipeline + hot reload + REPL | 166 tests verts | ✅ Complete |
| Phase 8 complete | 7 packages PokeForge.SDK.* publiables NuGet | 7 smoke tests verts, publish-nuget.yml prêt | ✅ Complete |
| v1.0 | dotnet add package PokeForge.SDK fonctionnel | Phase 9 (sample) restante | In Progress |

## Tech Stack

| Layer | Technology | Notes |
|-------|------------|-------|
| Runtime | .NET 10 | net10.0 partout |
| ORM | EF Core 10 + SQLite | Migrations obligatoires |
| Rendu | MonoGame.Framework.DesktopGL | OpenGL cross-platform |
| Tilemaps | MonoGame.Extended.Tiled | Import .tmx natif |
| Shaders | MojoShader intégré | HLSL→GLSL auto |
| Scripting | MoonSharp 2.0.0 | Pure C#, SoftSandbox |
| Tests | xUnit + FluentAssertions + Moq | coverlet.collector |
| DI | MS.Extensions.DependencyInjection | Composition root SDK.MonoGame |
| Logs | Serilog | Structured, sink fichier |
| JSON | System.Text.Json | Intégré .NET 10 |

---
*Created: 2026-06-01 | Last updated: 2026-06-07 after Phase 9 — v1.0 Milestone Complete*
*Full context: PROJECT.md | REQUIREMENTS.md | .claude/ARCHITECTURE.md | CLAUDE.md*
