# PokemonSDK

## What This Is

SDK open-source C# / .NET 10 pour fan-games Pokémon. Moteur de données SQLite (9 générations), battle engine headless, runtime MonoGame DesktopGL, système de plugins modulaire. Repo éditeur séparé : PokeForge-Editor (Avalonia).

## Core Value

Un développeur peut brancher ce SDK et obtenir immédiatement un moteur de combat, une base de données Pokémon multilingue et un système de quêtes fonctionnel — sans réimplémenter les règles de base.

## Current State

| Attribute | Value |
|-----------|-------|
| Type | Application |
| Version | 0.3.0 |
| Status | Phase 13 complete — BTLUI-02 EXP+LevelUp+Évolution shipped 2026-06-17 |
| Last Updated | 2026-06-17 — Phase 13 Evolution UI shipped |

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

### Shipped in Phase 10 ✅
- DX-05 — `PokeForge.CLI` global tool v0.1.0 : `pokeforge new` (scaffold StarterGame template), `asset-sync` (validate+pack+sync), `seed` (populate SQLite), `doctor` (health check headless D-17) — publish-cli.yml prêt pour tag `cli-v*.*.*` — *2026-06-09*

### Shipped in Phase 6 ✅
- ADV-03 — INarrationPlugin + PiperNarrationPlugin (piper + aplay) + TtsApi Lua binding `sdk.tts.speak/stop/is_speaking` + DoctorCommand TTS health check (piper+aplay WARN) — *2026-06-12*
- ADV-04 — FakemonSpecies entity (Migration AddFakemonSpecies, D-16 D-22) + FakemonAssemblyPipeline (catalog→filter→assemble→export) + `pokeforge fakemon list-parts/assemble` CLI — *2026-06-12*

### Shipped in Phase 11 ✅
- DX-06 — Docusaurus 3 documentation site EN+FR: Tutorial 30min (4 pages), Guides (8 pages, 7 subsystems), Packages/CLI/Advanced API reference. GitHub Pages CI (pages.yml). — *2026-06-13*

### Shipped in Phase 12 ✅
- BTLUI-01 — BattleScene UI: HP bars (lerp), sprites Pokémon, move menu FIGHT/RUN, flee auto-exit — *2026-06-14*

### Shipped in Phase 13 ✅
- BTLUI-02 — EXP gain + level-up overlay + move-learn overlay + evolution UI (flash/cancel/confirm) — *2026-06-17*

### Active (v1.0)
- Phase 14 : Items + Bag + Shop (BTLUI-03)

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
| v1.0 | dotnet add package PokeForge.SDK fonctionnel | StarterGame demo NuGet-only 09-04 ✅ | ✅ Complete 2026-06-07 |
| Phase 10 complete | `pokeforge new\|asset-sync\|seed\|doctor` CLI global tool | 13/13 tests verts, PokeForge.CLI.0.1.0.nupkg valide | ✅ Complete 2026-06-09 |
| Phase 6 complete | ADV-03 TTS + ADV-04 Fakemon assembly pipeline + CLI | 212/212 tests verts, `pokeforge fakemon assemble` fonctionnel | ✅ Complete 2026-06-12 |
| Phase 11 complete | Docusaurus docs EN+FR — Tutorial, Guides, Packages, CLI, Advanced | Build exit 0, 0 broken links, PR #27 → staging | ✅ Complete 2026-06-13 |
| v0.3 | CLI `pokeforge` + Docs Docusaurus + Advanced TTS/Fakemons | Phases 6+10+11 done, PR #27 open | ✅ Complete 2026-06-13 |

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
*Created: 2026-06-01 | Last updated: 2026-06-17 after Phase 13 — BTLUI-02 EXP+LevelUp+Évolution shipped*
*Full context: PROJECT.md | REQUIREMENTS.md | .claude/ARCHITECTURE.md | CLAUDE.md*
