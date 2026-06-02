# PokemonSDK

## What This Is

SDK open-source C# / .NET 10 pour fan-games Pokémon. Moteur de données SQLite (9 générations), battle engine headless, runtime MonoGame DesktopGL, système de plugins modulaire. Repo éditeur séparé : PokeForge-Editor (Avalonia).

## Core Value

Un développeur peut brancher ce SDK et obtenir immédiatement un moteur de combat, une base de données Pokémon multilingue et un système de quêtes fonctionnel — sans réimplémenter les règles de base.

## Current State

| Attribute | Value |
|-----------|-------|
| Type | Application |
| Version | 0.0.0 |
| Status | Phase 1 complete — Phase 2 (Battle Engine) starting |
| Last Updated | 2026-06-02 — Phase 1 shipped |

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

### Validated (À implémenter .NET 10)
- BATTLE-01→03, BATTLE-07 (moteur 1v1, formules, IA, config)
- SCRIPT-01→03 (MoonSharp, badges, save)

### Active (v0.1)
- Phase 1 : SDK.Core + SDK.Data
- Phase 2 : Battle Engine Core
- Phase 3 : World Foundation
- Phase 4 : Scripting + Progression

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
| Phase 2 complete | Combat 1v1 headless de start à KO | Not started | Not started |
| v0.1 | Joueur sur map, combat, badge, save/load | Not started | Not started |
| v1.0 | dotnet add package PokéForge.SDK fonctionnel | Not started | Not started |

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
*Created: 2026-06-01 | Last updated: 2026-06-02 after Phase 1*
*Full context: PROJECT.md | REQUIREMENTS.md | .claude/ARCHITECTURE.md | CLAUDE.md*
