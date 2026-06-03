---
phase: 02-battle-engine-core
plan: 02
subsystem: database
tags: [ef-core, sqlite, migration, seeder, type-chart, moves, abilities]

requires:
  - phase: 02-battle-engine-core plan 02-01
    provides: Move, Ability, Learnset entities + MoveCategory/WeatherType enums dans SDK.Core

provides:
  - Migration 002 AddBattleData — tables Moves, Abilities, Learnsets dans SQLite
  - MoveConfiguration, AbilityConfiguration, LearnsetConfiguration — Fluent API EF Core
  - BattleDataSeeder — tableau de types Gen 1 (83 entrées non-neutres), 15 moves Gen 1, 6 abilities
  - DataSeeder.SeedAll câblé sur BattleDataSeeder.SeedAll
  - BattleDataSeederTests — 4 tests validant counts et structure

affects: [02-03, 02-04, battle-engine, all-future-data-tests]

tech-stack:
  added: []
  patterns:
    - Pattern IDesignTimeDbContextFactory confirme migrations via --project src/SDK.Data seul (sans startup-project)
    - Seeder statique idempotent avec guard if Any() — réutilisable pour futurs seeders

key-files:
  created:
    - src/SDK.Data/Configurations/MoveConfiguration.cs
    - src/SDK.Data/Configurations/AbilityConfiguration.cs
    - src/SDK.Data/Configurations/LearnsetConfiguration.cs
    - src/SDK.Data/Migrations/20260603200241_AddBattleData.cs
    - src/SDK.Data/Seeding/BattleDataSeeder.cs
    - tests/SDK.Data.Tests/BattleDataSeederTests.cs
  modified:
    - src/SDK.Data/PokemonDbContext.cs
    - src/SDK.Data/Seeding/DataSeeder.cs
    - src/SDK.Data/Migrations/PokemonDbContextModelSnapshot.cs

key-decisions:
  - "dotnet ef migrations : --startup-project src/SDK.MonoGame inexistant → Pattern établi : --project src/SDK.Data seul suffit grâce à IDesignTimeDbContextFactory"
  - "Physical/Special catégorisé Gen 4+ sur moves Gen 1 — le battle engine (Plan 02-03) gèrera la logique Gen 1 (catégorie par type)"
  - "Gen 1 n'a pas de talents — Abilities seedées en Gen 3 (Overgrow/Blaze/Torrent/Static/Intimidate/Keen-Eye)"
  - "83 entrées TypeEffectiveness : seulement non-neutres (0/0.5/2) — défaut 1.0 implicite dans le battle engine"

patterns-established:
  - "Migration sans startup-project : dotnet ef migrations add <Name> --project src/SDK.Data"
  - "BattleDataSeeder en classe static séparée — ne pas surcharger DataSeeder.cs"

duration: ~15min
started: 2026-06-03T20:00:00Z
completed: 2026-06-03T20:15:00Z
---

# Phase 2 Plan 02 : SDK.Data Migration 002 + BattleDataSeeder

**Migration 002 ajoute Moves/Abilities/Learnsets à SQLite, BattleDataSeeder seed le tableau de types Gen 1 (83 entrées), 15 moves représentatifs et 6 abilities — 17/17 tests verts.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~15 min |
| Démarré | 2026-06-03T20:00Z |
| Complété | 2026-06-03T20:15Z |
| Tâches | 3/3 complétées |
| Fichiers créés | 6 |
| Fichiers modifiés | 3 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : Migration 002 crée Move/Ability/Learnset | Pass | Tables présentes dans PokemonSDK.db après `dotnet ef database update` |
| AC-2 : BattleDataSeeder seed correctement | Pass | 83 TypeEffectiveness Gen 1, 15 moves, 6 abilities — idempotent |
| AC-3 : Tests verts, zéro régression | Pass | 13/13 SDK.Data.Tests (dont 4 nouveaux) + 4/4 SDK.Core.Tests |

## Accomplishments

- **Migration 002** : tables Moves, Abilities, Learnsets créées avec FK correctes (Move.TypeId → PokemonTypes, Learnset → Species + Move)
- **BattleDataSeeder** : tableau de types Gen 1 complet (15 types, 83 entrées non-neutres), 15 moves Gen 1 (Physical/Special/Status), 6 abilities Gen 3
- **4 nouveaux tests** : BattleDataSeederTests valide counts, catégories, spot-check Tackle — pattern réutilisable
- **Pattern migrations établi** : `dotnet ef migrations add <Name> --project src/SDK.Data` — sans startup-project grâce à IDesignTimeDbContextFactory

## Commit

| Scope | Commit | Description |
|-------|--------|-------------|
| Task 1+2+3 | `3337ee8` | feat(data): Migration 002 + BattleDataSeeder + type chart Gen 1 |

## Fichiers Créés / Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Data/Configurations/MoveConfiguration.cs` | Créé | Fluent API Move — FK TypeId, Category enum→int, Power nullable |
| `src/SDK.Data/Configurations/AbilityConfiguration.cs` | Créé | Fluent API Ability — Identifier unique |
| `src/SDK.Data/Configurations/LearnsetConfiguration.cs` | Créé | Fluent API Learnset — FK Species+Move, index (SpeciesId, Generation) |
| `src/SDK.Data/Migrations/20260603200241_AddBattleData.cs` | Créé | Migration 002 — CreateTable Moves/Abilities/Learnsets |
| `src/SDK.Data/Seeding/BattleDataSeeder.cs` | Créé | Seeder battle data — SeedTypeEffectiveness/Moves/Abilities + SeedAll |
| `tests/SDK.Data.Tests/BattleDataSeederTests.cs` | Créé | 4 tests : counts TypeEffectiveness, catégories moves, Tackle spot-check, abilities |
| `src/SDK.Data/PokemonDbContext.cs` | Modifié | +3 DbSets : Moves, Abilities, Learnsets |
| `src/SDK.Data/Seeding/DataSeeder.cs` | Modifié | SeedAll appelle BattleDataSeeder.SeedAll |
| `src/SDK.Data/Migrations/PokemonDbContextModelSnapshot.cs` | Modifié | Snapshot auto-régénéré par EF Core |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `--project src/SDK.Data` sans startup-project | SDK.MonoGame inexistant (Phase 3), IDesignTimeDbContextFactory dans SDK.Data suffit | Pattern établi pour toutes futures migrations |
| Catégorie Physical/Special Gen 4+ sur moves Gen 1 | Gen 1 utilisait catégorie par type — simplification pour le seeder | Plan 02-03 implémentera la logique Gen 1 dans le battle engine |
| Abilities Gen 3 (pas Gen 1) | Gen 1 n'avait pas de système de talents | Normal et attendu — les données abilities sont disponibles pour Gen 3+ |
| 83 entrées TypeEffectiveness (non-neutres seulement) | Défaut 1.0 implicite — chart sparse plus performant | Plan 02-03 : requête "if no row → factor = 1.0" |

## Déviations du Plan

**1. Startup project manquant pour EF migrations**
- **Trouvé pendant :** Task 1
- **Problème :** `--startup-project src/SDK.MonoGame` échoue (projet Phase 3, pas encore créé). `--startup-project src/SDK.Tools` échoue (pas de EF Design)
- **Fix :** `dotnet ef migrations add AddBattleData --project src/SDK.Data` — IDesignTimeDbContextFactory dans SDK.Data suffit
- **Impact :** Aucun sur le code produit. Pattern établi pour Plans 02-03/03-01/04-02

## Readiness pour Plan 02-03

**Prêt :**
- Tables Moves, Abilities, Learnsets accessibles via `ctx.Moves`, `ctx.Abilities`, `ctx.Learnsets`
- 15 moves Gen 1 disponibles pour tester le battle engine
- 83 entrées TypeEffectiveness Gen 1 — tableau complet pour formule de dégâts
- Pattern migration établi : `dotnet ef --project src/SDK.Data`

**Déférés à surveiller :**
- Learnset seeding (Bulbasaur/Pikachu/Togepi ↔ moves) → Plan 02-03 ou 02-04
- Translations Move/Ability (D-22) → phase future (DX ou post-v0.1)
- Seed CLI (`src/SDK.Tools`) ne lance pas BattleDataSeeder automatiquement — à vérifier

**Blockers :**
Aucun.

---
*Phase: 02-battle-engine-core, Plan: 02*
*Complété: 2026-06-03*
