---
phase: 07-developer-experience
plan: 02
subsystem: tooling
tags: [atlas-packer, sqlite-syncer, asset-sync, imagesharp, sdk-tools, cli]

requires:
  - phase: 07-01
    provides: SpriteScanner + SpriteValidator + CLI asset-validate

provides:
  - AtlasPacker (PNG atlas + import.json via ImageSharp)
  - SqliteSyncer (import.json → PokemonSDK.db via EF Core)
  - CLI asset-sync (pipeline filter→pack→sync)

affects: [phase8-nuget, phase9-sample]

tech-stack:
  added: [SixLabors.ImageSharp 2.1.9 (Apache 2.0) dans SDK.Tools]
  patterns: [CLI pipeline filter ERROR→pack OK+WARN→sync, exit code 1 si au moins 1 ERROR]

key-files:
  created:
    - src/SDK.Tools/Atlas/AtlasPacker.cs
    - src/SDK.Tools/Atlas/AtlasEntry.cs
    - src/SDK.Tools/Sync/SqliteSyncer.cs
    - src/SDK.Tools/Sync/ImportManifest.cs
    - tests/SDK.Tools.Tests/AtlasPackerTests.cs
    - tests/SDK.Tools.Tests/SqliteSyncerTests.cs
  modified:
    - src/SDK.Tools/Program.cs
    - src/SDK.Tools/SDK.Tools.csproj

key-decisions:
  - "D-25 : SixLabors.ImageSharp 2.1.9 (Apache 2.0) — v4.0.0 impose licence commerciale MSBuild"
  - "AtlasPacker tests : vrais PNG via Image<Rgba32>.SaveAsPng — Image.Load() valide headers complets"
  - "SqliteSyncer test fixture : temp file .db (pas :memory:) — constructeur prend dbPath string"
  - "CLI asset-sync : filter ERROR→pack OK+WARN→sync, exit code 1 si au moins 1 ERROR"

patterns-established:
  - "CLI pipeline linéaire : validate → pack → sync avec early exit sur ERROR"

duration: ~50min
started: 2026-06-06T20:05:00Z
completed: 2026-06-06T20:55:00Z
---

# Phase 7 Plan 02: AtlasPacker + SqliteSyncer + CLI asset-sync Summary

**AtlasPacker génère atlas PNG + import.json via ImageSharp 2.1.9, SqliteSyncer importe l'atlas en DB, CLI asset-sync orchestre le pipeline complet — DX-01 livré.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~50min |
| Started | 2026-06-06T20:05:00Z |
| Completed | 2026-06-06T20:55:00Z |
| Tasks | 3 completed |
| Files modified | 8 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: AtlasPacker génère atlas PNG + import.json | Pass | ImageSharp 2.1.9, atlas rectangulaire |
| AC-2: SqliteSyncer importe import.json → DB | Pass | EF Core, idempotent sur re-run |
| AC-3: CLI asset-sync pipeline complet | Pass | filter→pack→sync, exit code 1 si ERROR |
| AC-4: Tests SDK.Tools.Tests verts | Pass | Tests AtlasPacker + SqliteSyncer verts |

## Accomplishments

- Pipeline complet DX-01 : sprites validés → atlas généré → DB peuplée en une commande
- D-25 confirmé : ImageSharp 2.1.9 Apache 2.0 (v4.0.0 évitée — licence commerciale MSBuild)
- CVE SixLabors.ImageSharp 2.x documentés dans Deferred Issues (outil interne, input trusted)

## Task Commits

| Task | Commit | Type | Description |
|------|--------|------|-------------|
| T0+T1+T2: Full pipeline | `54036b8` | feat | AtlasPacker + SqliteSyncer + CLI asset-sync |

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Tools/Atlas/AtlasPacker.cs` | Created | Pack PNG sprites en atlas + génère import.json |
| `src/SDK.Tools/Atlas/AtlasEntry.cs` | Created | Record entry atlas (dexid, x, y, w, h) |
| `src/SDK.Tools/Sync/SqliteSyncer.cs` | Created | Importe import.json dans PokemonSDK.db |
| `src/SDK.Tools/Sync/ImportManifest.cs` | Created | DTO import.json désérialisé |
| `src/SDK.Tools/Program.cs` | Modified | Branche asset-sync + pipeline orchestration |
| `src/SDK.Tools/SDK.Tools.csproj` | Modified | ImageSharp 2.1.9 ajouté |
| `tests/SDK.Tools.Tests/AtlasPackerTests.cs` | Created | Tests AtlasPacker avec vrais PNG |
| `tests/SDK.Tools.Tests/SqliteSyncerTests.cs` | Created | Tests SqliteSyncer avec temp .db |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| ImageSharp 2.1.9 (pas v4.x) | v4.0.0 impose licence commerciale au MSBuild build — 2.1.9 Apache 2.0 | D-25 confirmé, CVE à tracker avant Phase 8 |
| Tests AtlasPacker : Image<Rgba32>.SaveAsPng | Image.Load() valide headers complets — synthétique 26 bytes insuffisant | Pattern distinct de SpriteValidatorTests |
| SqliteSyncer : dbPath string (pas DbContext) | Constructeur simple, testable avec temp file sans DI | Cohérent avec SDK.Data design |
| Pipeline : filter ERROR → early exit | Pas d'atlas si sprites invalides — cohérence données | Exit code 1 intégrable CI |

## Deferred Items

- CVE SixLabors.ImageSharp 2.1.9 (GHSA-2cmq-823j-5qj8 high + GHSA-rxmq-m78w-7wmc moderate) → envisager 3.x Community License avant Phase 8 NuGet distribution (outil interne SDK.Tools, input trusted, risque faible en l'état)

## Next Phase Readiness

**Ready:**
- DX-01 complet — pipeline sprite validate → atlas → DB opérationnel
- SDK.Tools.Tests enrichi avec AtlasPacker et SqliteSyncer

**Concerns:**
- ImageSharp CVE à résoudre avant distribution NuGet (Phase 8)

**Blockers:** None

---
*Phase: 07-developer-experience, Plan: 02*
*Completed: 2026-06-06*
