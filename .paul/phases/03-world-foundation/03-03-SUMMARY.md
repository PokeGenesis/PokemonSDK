---
phase: 03-world-foundation
plan: 03
subsystem: rendering
tags: [monogame, desktopgl, renderpipeline, worldsystem, playersystem, headless, di]

requires:
  - phase: 03-02
    provides: IGameClock, IWeatherSystem, IEncounterSystem, GameTimeClock, WeatherSystem, EncounterSystem

provides:
  - SDK.MonoGame project (exécutable, composition root DI)
  - RenderPipeline 480×270 → xBR × 4 → 1920×1080 (D-14, D-15)
  - HeadlessRunner — loop Update() sans GL context (base Plan 03-04 tests)
  - WorldSystem wrappant IGameClock + IEncounterSystem + IWeatherSystem
  - PlayerSystem tile-based (TileSize=16, D-14)
  - IInputProvider + NullInputProvider + KeyboardInputProvider

affects: [03-04, phase-4-scripting, phase-5-plugins]

tech-stack:
  added:
    - MonoGame.Framework.DesktopGL 3.8.4.1
    - Microsoft.Extensions.DependencyInjection 10.0.8
    - Serilog 4.2.0 + Sinks.File 7.0.0 + Sinks.Console 6.1.1
  patterns:
    - HeadlessRunner pattern — Update() loop sans SDL/GL pour tests xUnit
    - RenderPipeline 3-pass — internalRT / xBR upscale / DayNight tint
    - IInputProvider action-string abstraction — découple input du système
    - WorldSystem.Update() wrappe clock.Update() — HeadlessRunner ne touche pas clock directement

key-files:
  created:
    - src/SDK.Core/Interfaces/IInputProvider.cs
    - src/SDK.MonoGame/SDK.MonoGame.csproj
    - src/SDK.MonoGame/Program.cs
    - src/SDK.MonoGame/HeadlessRunner.cs
    - src/SDK.MonoGame/Game1.cs
    - src/SDK.MonoGame/Rendering/RenderPipeline.cs
    - src/SDK.MonoGame/Rendering/Camera2D.cs
    - src/SDK.MonoGame/Rendering/TilemapRenderer.cs
    - src/SDK.MonoGame/Rendering/Shaders/xBR.fx
    - src/SDK.MonoGame/Rendering/Shaders/DayNight.fx
    - src/SDK.MonoGame/World/WorldSystem.cs
    - src/SDK.MonoGame/World/PlayerSystem.cs
    - src/SDK.MonoGame/Input/KeyboardInputProvider.cs
    - src/SDK.MonoGame/Input/NullInputProvider.cs
    - src/SDK.MonoGame/Content/Content.mgcb
  modified:
    - PokemonSDK.slnx

key-decisions:
  - "WorldSystem.Update() appelle clock.Update() en interne — HeadlessRunner n'appelle que world.Update()"
  - "NullInputProvider enregistré via DI quand --headless — même interface, zéro input"
  - "MonoGame.Extended entièrement absent — TilemapRenderer stub, ajout Plan 03-04 avec vérification compat"
  - "DB path = 'Data Source=src/SDK.Data/data/PokemonSDK.db' relatif repo root — toujours lancer depuis racine"
  - "MS.DI 10.0.8 (pas 10.0.0) — EF Core 10.0.8 impose cette contrainte transitive"

patterns-established:
  - "HeadlessRunner: static class, IServiceProvider injection, 0 types MonoGame — base des tests Plan 03-04"
  - "RenderPipeline null-safe: try/catch sur content.Load<Effect>() — jeu ne crash pas si shader absent"
  - "IInputProvider action-string: 'Up'/'Down'/'Left'/'Right'/'A'/'B'/'Start' — extensible sans toucher les systèmes"

duration: ~90min
started: 2026-06-05T18:00:00Z
completed: 2026-06-05T20:10:00Z
---

# Phase 3 Plan 03: SDK.MonoGame Scaffold — Summary

**SDK.MonoGame créé de zéro — Game1 (MonoGame.DesktopGL), RenderPipeline 480×270 → xBR → 1080p, WorldSystem/PlayerSystem wirés sur les interfaces Phase 3-02, HeadlessRunner pour CI headless — 0 erreur build, 74/74 tests verts.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~90 min |
| Démarré | 2026-06-05T18:00:00Z |
| Complété | 2026-06-05T20:10:00Z |
| Tasks | 3/3 |
| Fichiers créés | 15 |
| Fichiers modifiés | 1 (PokemonSDK.slnx) |
| Insertions | 431 lignes |
| Commit | `8676cc5` |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : SDK.MonoGame compile, D-02 respecté, ajouté à solution | Pass | DesktopGL ✓, 0 ref WindowsDX ✓, 0 ref SDK.Scripting ✓ |
| AC-2 : HeadlessRunner — N frames sans GraphicsDevice | Pass | Loop pure C#, NullInputProvider, 60 frames par défaut |
| AC-3 : RenderPipeline D-14 (480×270) + D-15 (xBR stub) | Pass | RenderTarget2D(gd, 480, 270) ✓, 3 passes ✓, no-op headless ✓ |
| AC-4 : WorldSystem + PlayerSystem wirés IGameClock / IEncounterSystem / IWeatherSystem | Pass | WorldSystem(clock, enc, wx) ✓, update interne clock ✓ |
| AC-5 : Build 0 erreur + 74 tests verts | Pass | `dotnet build` 0 erreur 0 warning, 74/74 green |

## Accomplissements

- Projet `SDK.MonoGame` créé et intégré à `PokemonSDK.slnx` — 6 projets source désormais
- `IInputProvider` ajouté à `SDK.Core.Interfaces` — pure interface, 0 NuGet dans SDK.Core (règle D-01)
- `RenderPipeline` : RenderTarget2D 480×270 (D-14), xBR passthrough stub (D-15), tints DayNight par Color multiplication, null-safe shader loading, no-op total en mode headless
- `HeadlessRunner` : classe statique, n'étend pas MonoGame.Game, 0 appel SDL/GL — Plan 03-04 écrit des tests xUnit qui l'appellent directement
- `WorldSystem` : pilote IGameClock en interne (pattern établi), query IWeatherSystem par biome/timeOfDay, CheckWildEncounter() sur zone courante
- `PlayerSystem` : mouvement tile-aligned TileSize=16 (D-14), fire-and-forget CheckWildEncounter() à chaque pas

## Task Commits

| Task | Commit | Description |
|------|--------|-------------|
| T1 IInputProvider + scaffold | `8676cc5` | IInputProvider, SDK.MonoGame.csproj, Program.cs, HeadlessRunner, Content.mgcb, slnx |
| T2 Game1 + RenderPipeline | `8676cc5` | Game1, RenderPipeline, Camera2D, TilemapRenderer stub, xBR.fx, DayNight.fx |
| T3 WorldSystem + Input | `8676cc5` | WorldSystem, PlayerSystem, KeyboardInputProvider, NullInputProvider |

*(Commit unique pour les 3 tasks — pattern établi Plan 03-02)*

## Fichiers Créés/Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Core/Interfaces/IInputProvider.cs` | Créé | Interface action-string, SDK.Core zéro NuGet |
| `src/SDK.MonoGame/SDK.MonoGame.csproj` | Créé | Exécutable net10.0, DesktopGL 3.8.4.1, DI/Serilog |
| `src/SDK.MonoGame/Program.cs` | Créé | Composition root DI, bootstrap --headless / --max-frames |
| `src/SDK.MonoGame/HeadlessRunner.cs` | Créé | Loop 60fps sans GL, base tests Plan 03-04 |
| `src/SDK.MonoGame/Game1.cs` | Créé | MonoGame.Game, IServiceProvider injection, 1920×1080 windowed |
| `src/SDK.MonoGame/Rendering/RenderPipeline.cs` | Créé | 3-pass pipeline 480×270 → xBR → DayNight, no-op headless |
| `src/SDK.MonoGame/Rendering/Camera2D.cs` | Créé | Follow() Lerp 0.12f, GetVisibleBounds() cull margin 32px |
| `src/SDK.MonoGame/Rendering/TilemapRenderer.cs` | Créé | Stub — TMX loading Plan 03-04+ |
| `src/SDK.MonoGame/Rendering/Shaders/xBR.fx` | Créé | HLSL passthrough stub ps_3_0, hors Content.mgcb |
| `src/SDK.MonoGame/Rendering/Shaders/DayNight.fx` | Créé | HLSL tint stub ps_3_0, hors Content.mgcb |
| `src/SDK.MonoGame/World/WorldSystem.cs` | Créé | IGameClock + IWeatherSystem + IEncounterSystem wrapping |
| `src/SDK.MonoGame/World/PlayerSystem.cs` | Créé | Tile-based (TileSize=16), IInputProvider injection |
| `src/SDK.MonoGame/Input/KeyboardInputProvider.cs` | Créé | Arrow+WASD, Z/X/Esc, edge detection prev/current state |
| `src/SDK.MonoGame/Input/NullInputProvider.cs` | Créé | Null object headless/test — toujours false |
| `src/SDK.MonoGame/Content/Content.mgcb` | Créé | Manifest vide — shaders ajoutés Plan 03-04 CI |
| `PokemonSDK.slnx` | Modifié | +1 ligne `<Project Path="src/SDK.MonoGame/SDK.MonoGame.csproj" />` |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `WorldSystem.Update()` appelle `clock.Update()` en interne | HeadlessRunner n'a besoin que de `world.Update(delta)` — DI plus propre | Plan 03-04 tests : 1 appel par frame, pas 2 |
| `NullInputProvider` dans DI quand `--headless` | Même interface, 0 branchement conditionnel dans PlayerSystem | Testabilité sans mock |
| `MonoGame.Extended` entièrement supprimé | Package ne compile pas avec MonoGame 3.8.4.1 — compat non vérifiée | TilemapRenderer reste stub jusqu'à Plan 03-04 |
| `MS.DI 10.0.8` (pas 10.0.0 du plan) | NU1605 : EF Core 10.0.8 impose `>= 10.0.8` transitivement | Build propre, 0 warning NuGet |
| Shaders `.fx` hors `Content.mgcb` | MGCB pas configuré en CI avant Plan 03-04 — évite build error | Jeu démarre même sans shader compilé (try/catch) |

## Déviations du Plan

### Résumé

| Type | Nb | Impact |
|------|-----|--------|
| Auto-fixées | 2 | Minimes — package downgrade + clockUpdate coupling |
| Scope additions | 1 | NullInputProvider ajouté (non listé frontmatter plan) |
| Déférés | 1 | MonoGame.Extended reporté Plan 03-04 |

**Impact total :** Déviations essentielles, 0 scope creep.

### Auto-fixées

**1. Package Downgrade — MS.DI 10.0.0 → 10.0.8**
- **Trouvé lors :** Task 1 (build)
- **Problème :** `error NU1605` — EF Core 10.0.8 exige MS.DI ≥ 10.0.8
- **Fix :** `Version="10.0.8"` dans SDK.MonoGame.csproj
- **Vérification :** `dotnet build` 0 NU1605

**2. WorldSystem wrapping clock — HeadlessRunner simplifié**
- **Trouvé lors :** Task 1 (architecture)
- **Plan :** HeadlessRunner appelait `clock.Update()` + `world.Update()` séparément
- **Réel :** WorldSystem.Update() appelle `_clock.Update(delta)` en interne — HeadlessRunner appelle seulement `world.Update(delta)`
- **Rationale :** Plus cohérent — Game1 et HeadlessRunner ont le même point d'entrée unique

### Ajout de scope

**NullInputProvider** (`src/SDK.MonoGame/Input/NullInputProvider.cs`) — non listé dans `files_modified` du plan mais requis pour wirer `IInputProvider` en mode headless. Ajout évident, 9 lignes.

### Déféré

**MonoGame.Extended (TilemapRenderer)** — Le plan listait `MonoGame.Extended 4.0.2` et `MonoGame.Extended.Tiled 4.0.2`. Ces packages n'existent plus sur NuGet (fusionnés dans MonoGame.Extended 6.0.0, compat MonoGame 3.8.4.1 non vérifiée). TilemapRenderer est un stub pur. Plan 03-04 vérifie la compatibilité et intègre si possible.

## Issues Rencontrées

| Issue | Résolution |
|-------|-----------|
| `NU1605` MS.DI downgrade | Version bumped 10.0.0 → 10.0.8 |
| `MSB3492` CoreCompileInputs.cache (WSL2 race) | `rm -rf <project>/obj/` + rebuild — pattern établi Plans 03-01/02 |
| `dotnet build -q` false positives | Confirmé : `-q` expose diagnostics MSBuild internes ; run sans `-q` = 0 erreur réelle |
| MonoGame.Extended.Tiled inexistant | Skipped entièrement, stub TilemapRenderer |

## Next Phase Readiness

**Prêt :**
- `HeadlessRunner.Run(sp, N)` — appelable depuis tests xUnit sans GL context
- `IInputProvider` / `NullInputProvider` — mock-ready pour tests
- `WorldSystem` + `PlayerSystem` — Update() loops stables
- `RenderPipeline` — structure 3-pass en place, slots shader définis
- `Program.cs` — `--headless --max-frames=N` parsé

**Concerns :**
- `MonoGame.Extended` entièrement absent — Plan 03-04 doit vérifier compat avant d'activer TilemapRenderer
- Shaders `.fx` non compilés — `Content.mgcb` vide jusqu'à Plan 03-04 CI MGCB
- `Game1.Draw()` récupère `IGameClock` via service locator (non stocké en champ) — à refactoriser si perf critique (Plan futur)
- `KeyboardInputProvider.IsActionJustPressed()` a un workaround interne — revoir à Plan 03-04 si edge detection défaillante

**Blockers :**
- Aucun pour Plan 03-04.

---
*Phase: 03-world-foundation, Plan: 03*
*Complété: 2026-06-05*
