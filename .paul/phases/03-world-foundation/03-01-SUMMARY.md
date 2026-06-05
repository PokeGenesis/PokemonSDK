---
phase: 03-world-foundation
plan: 01
subsystem: data
tags: [ef-core, sqlite, migration, encounter-zone, translations, d-22, seeder]

requires:
  - phase: 02-battle-engine-core (plan 03+04)
    provides: BattleDataSeeder.SeedMoves + SeedAbilities (15 moves, 6 abilities — à translater)
  - phase: 01-sdk-core-data (plan 01-02)
    provides: IDesignTimeDbContextFactory pattern, SqliteTestFixture :memory:, PokemonDbContext

provides:
  - EncounterZone entity (SDK.Core) — prêt pour IEncounterSystem implem Plan 03-02
  - BiomeType enum (SDK.Core) — Grass/Cave/Water/Building/Route
  - IEncounterSystem interface (SDK.Core) — stub pour Plan 03-02
  - Migration 003 AddWorldData — table encounter_zones en DB
  - SeedMoveTranslations — 15 moves × 6 locales = 90 rows (D-22 compliant)
  - SeedAbilityTranslations — 6 abilities × 6 locales = 36 rows (D-22 compliant)
  - BattleTranslationsD22Tests — 2 tests vérifiant les counts de translations

affects: [plan 03-02 EncounterSystem, plan 03-03 WorldSystem, plan 04-02 migrations]

tech-stack:
  added: []
  patterns:
    - "Migration --startup-project src/SDK.Data (SDK.MonoGame absent jusqu'en Plan 03-03)"
    - "SeedMoveTranslations / SeedAbilityTranslations suivent exactement le pattern SeedTypeTranslations"
    - "EntityType = 'Move' / 'Ability' — casse PascalCase comme PokemonType / PokemonSpecies"

key-files:
  created:
    - src/SDK.Core/Entities/EncounterZone.cs
    - src/SDK.Core/Enums/BiomeType.cs
    - src/SDK.Core/Interfaces/IEncounterSystem.cs
    - src/SDK.Data/Configurations/EncounterZoneConfiguration.cs
    - src/SDK.Data/Migrations/20260604195701_AddWorldData.cs
    - tests/SDK.Data.Tests/BattleTranslationsD22Tests.cs
  modified:
    - src/SDK.Data/PokemonDbContext.cs (DbSet<EncounterZone> ajouté)
    - src/SDK.Data/Seeding/BattleDataSeeder.cs (SeedMoveTranslations + SeedAbilityTranslations)

key-decisions:
  - "--startup-project src/SDK.Data pour ef migrations (SDK.MonoGame pas encore créé)"
  - "EncounterZone contient SpeciesId FK direct — une row par espèce/zone/niveau (pas de table join séparée)"

patterns-established:
  - "SeedXTranslations pattern établi dans BattleDataSeeder — réutilisable Plan 03-02 (zones) et au-delà"

duration: ~30min
started: 2026-06-04T20:00:00Z
completed: 2026-06-04T20:30:00Z
---

# Phase 3 Plan 01: Migration 003 + SDK.Core World Primitives + Translations D-22

**EncounterZone entity + table encounter_zones + 126 translations Move/Ability (D-22 comblé) — 49/49 tests verts.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~30 min |
| Started | 2026-06-04T20:00Z |
| Completed | 2026-06-04T20:30Z |
| Tasks | 3/3 complétées |
| Files modifiés | 8 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: EncounterZone world primitives dans SDK.Core sans dépendance externe | Pass | `dotnet list SDK.Core.csproj package` → 0 résultats, 4/4 Core tests verts |
| AC-2: Table encounter_zones présente après Migration 003 | Pass | `sqlite3 .tables` inclut EncounterZones, 47 tests existants toujours verts |
| AC-3: Translations D-22 — 15×6=90 Move + 6×6=36 Ability | Pass | BattleTranslationsD22Tests 2/2 verts, 49/49 total |

## Accomplishments

- EncounterZone entity + BiomeType + IEncounterSystem dans SDK.Core — fondations Plan 03-02 prêtes
- Migration 003 AddWorldData appliquée — table `encounter_zones` en SQLite
- D-22 comblé : 90 translations Move + 36 translations Ability seed via BattleDataSeeder
- Pattern `SeedXTranslations` établi — réutilisable pour futures entités (Items, Routes, etc.)

## Task Commits

| Tâche | Commit | Description |
|-------|--------|-------------|
| T1+T2+T3 | `703e4a5` | feat(world): Plan 03-01 — EncounterZone + Migration 003 + translations D-22 |

## Files Created/Modified

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Core/Entities/EncounterZone.cs` | Créé | Entité zone de rencontre (Id, ZoneIdentifier, Generation, BiomeType, SpeciesId, MinLevel, MaxLevel, SpawnRate) |
| `src/SDK.Core/Enums/BiomeType.cs` | Créé | Enum Grass/Cave/Water/Building/Route |
| `src/SDK.Core/Interfaces/IEncounterSystem.cs` | Créé | Interface stub `GetZones(int generation)` |
| `src/SDK.Data/Configurations/EncounterZoneConfiguration.cs` | Créé | EF Fluent API — PK, ZoneIdentifier maxLen 100, Generation D-09, BiomeType→int, SpawnRate REAL, FK Restrict |
| `src/SDK.Data/Migrations/20260604195701_AddWorldData.cs` | Créé | Migration 003 — CREATE TABLE EncounterZones |
| `src/SDK.Data/PokemonDbContext.cs` | Modifié | Ajout `DbSet<EncounterZone> EncounterZones` |
| `src/SDK.Data/Seeding/BattleDataSeeder.cs` | Modifié | Ajout SeedMoveTranslations (15 moves × 6 locales) + SeedAbilityTranslations (6 abilities × 6 locales), wiring dans SeedAll |
| `tests/SDK.Data.Tests/BattleTranslationsD22Tests.cs` | Créé | 2 tests D-22 : Move translations count==90, Ability translations count==36 |

## Decisions Made

| Décision | Raison | Impact |
|----------|--------|--------|
| `--startup-project src/SDK.Data` pour ef migrations | SDK.MonoGame pas encore créé (Plan 03-03) | À jour dans STATE.md decisions émergentes |
| EncounterZone avec SpeciesId FK direct (une row par espèce) | Modèle table de rencontre simple — une row = "dans zone X, espèce Y spawn niveau MinLevel-MaxLevel à SpawnRate%" | Plan 03-02 peut étendre ou ajouter une table join si nécessaire |

## Deviations from Plan

| Type | Détail | Impact |
|------|--------|--------|
| Auto-fix | `--startup-project src/SDK.MonoGame` → `src/SDK.Data` | Nul — SDK.Data contient IDesignTimeDbContextFactory, migration générée correctement |

## Issues Encountered

| Problème | Résolution |
|----------|-----------|
| MSBuild transient cache error (CoreCompileInputs.cache) après rm -rf obj | Second build toujours résout — erreur bénigne, pattern récurrent dans cette session |

## Next Phase Readiness

**Prêt :**
- `EncounterZone` entity + `IEncounterSystem` interface → Plan 03-02 peut implémenter EncounterSystem
- Table `encounter_zones` en DB → Plan 03-02 peut seeder des données de zones
- Translations D-22 complètes pour les entités actuelles (Types, Species, Moves, Abilities)

**Concerns :**
- SDK.MonoGame n'existe pas encore — Plans 03-02 et 03-03 doivent y remédier
- EF migrations continueront d'utiliser `--startup-project src/SDK.Data` jusqu'à Plan 03-03

**Blockers :** Aucun

---
*Phase: 03-world-foundation, Plan: 01*
*Completed: 2026-06-04*
