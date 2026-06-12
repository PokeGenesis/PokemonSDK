---
phase: 06-advanced-systems
plan: 01
subsystem: database
tags: [ef-core, sqlite, fakemon, sprite-validation, seeding, d22]

requires:
  - phase: 05-plugins-characters
    provides: SDK.Core.Entities base pattern, PokemonTypes table (IDs stable)

provides:
  - FakemonSpecies entity (SDK.Core.Entities)
  - fakemon_species SQLite table via EF Core migration
  - FakemonDataSeeder with 6-locale D-22 translations
  - SpriteValidator extended with FakemonPattern regex (fk_ prefix)
  - REQUIREMENTS.md ADV-03/ADV-04 corrected to v0.3
  - CLAUDE.md D-16 updated with fk_ naming convention

affects: [06-02-fakemon-assembly, 06-03-fakemon-cli]

tech-stack:
  added: []
  patterns:
    - "FakemonPattern regex séparé de D16Pattern — deux regex, groupes distincts, ParseEntry try D16 then Fakemon"
    - "SpriteEntry.DexId = null pour Fakemons — nullable déjà prévu dans l'entité"
    - "EggGroup comme TEXT string — pas de FK table, simplification délibérée"

key-files:
  created:
    - src/SDK.Core/Entities/FakemonSpecies.cs
    - src/SDK.Data/Configurations/FakemonSpeciesConfiguration.cs
    - src/SDK.Data/Migrations/20260612162309_AddFakemonSpecies.cs
    - src/SDK.Data/Seeding/FakemonDataSeeder.cs
  modified:
    - REQUIREMENTS.md
    - CLAUDE.md
    - src/SDK.Tools/Validation/SpriteValidator.cs
    - src/SDK.Data/PokemonDbContext.cs

key-decisions:
  - "EggGroup1/2 = TEXT NOT NULL/NULL — pas de table egg_groups (simplification Phase 6)"
  - "Deux regex SpriteValidator: D16Pattern + FakemonPattern — préserve mapping groupes existant"
  - "Dragon TypeId = 15 — utilisé pour seed test-dragon"
  - "Config dans SDK.Data/Configurations/ (pas Models/) — auto-scan ApplyConfigurationsFromAssembly"

patterns-established:
  - "FakemonSpecies suit même pattern que PokemonSpecies : Id, Identifier, Generation, BaseStats, Type FKs"
  - "FakemonDataSeeder.Seed() guard Any() double — protège FakemonSpecies + Translations séparément"

duration: ~40min
started: 2026-06-12T16:08:00Z
completed: 2026-06-12T16:25:00Z
---

# Phase 6 Plan 01: Doc Fixes + FakemonSpecies Entity + Migration — Summary

**FakemonSpecies entity (15 colonnes, 2 FK PokemonTypes) + migration EF Core + seeder D-22 + SpriteValidator étendu pour sprites fk_.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~40 min |
| Démarré | 2026-06-12T16:08Z |
| Terminé | 2026-06-12T16:25Z |
| Tâches | 2/2 complètes |
| Fichiers modifiés | 8 (4 créés + 4 modifiés) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: REQUIREMENTS.md ADV-03/ADV-04 corrigé | ✅ PASS | v2.0 → v0.3 ; Post-v1.0 → v0.3 / Phase 6 |
| AC-2: CLAUDE.md D-16 + SpriteValidator mis à jour | ✅ PASS | D-16 documente fk_ ; FakemonPattern ajouté dans SpriteValidator |
| AC-3: Migration AddFakemonSpecies appliquée | ✅ PASS | `20260612162309_AddFakemonSpecies` — table fakemon_species créée, 2 FKs Restrict |
| AC-4: FakemonDataSeeder D-22 valide | ✅ PASS | "test-dragon" (Type1Id=15) + 6 translations EntityType="FakemonSpecies" |

## Accomplissements

- `FakemonSpecies` entity : Id, Identifier (unique index), Generation, 6 BaseStats, Type1Id/2Id (FK Restrict), EggGroup1/2 TEXT, IsLegendary, PartsManifest nullable TEXT
- Migration SQLite générée et appliquée : table `fakemon_species` avec contraintes complètes
- `FakemonDataSeeder` : seed test-dragon + 6 translations en/es/fr/de/it/ja (D-22 complet)
- `SpriteValidator` : `FakemonPattern` `^(fk_[a-z0-9-]+)_(front|back|...)\.png$` ; `D16Pattern` charset étendu avec `-` ; `ParseEntry` try D16 first, then Fakemon fallback
- Docs corrigées : REQUIREMENTS.md ADV-03/04 ciblage v0.3 ; CLAUDE.md D-16 regex documentée

## Fichiers Créés/Modifiés

| Fichier | Changement | But |
|---------|-----------|-----|
| `src/SDK.Core/Entities/FakemonSpecies.cs` | Créé | Entité FakemonSpecies — EggGroup TEXT, PartsManifest JSON |
| `src/SDK.Data/Configurations/FakemonSpeciesConfiguration.cs` | Créé | Fluent API — table fakemon_species, unique Identifier, 2 FKs |
| `src/SDK.Data/Migrations/20260612162309_AddFakemonSpecies.cs` | Créé | Migration EF Core + Designer |
| `src/SDK.Data/Seeding/FakemonDataSeeder.cs` | Créé | Seed "test-dragon" + 6 translations D-22 |
| `REQUIREMENTS.md` | Modifié | ADV-03/ADV-04 : v2.0 → v0.3 |
| `CLAUDE.md` | Modifié | D-16 : ajout pattern fk_ + regex SpriteValidator |
| `src/SDK.Tools/Validation/SpriteValidator.cs` | Modifié | FakemonPattern regex + ParseEntry two-pass |
| `src/SDK.Data/PokemonDbContext.cs` | Modifié | `DbSet<FakemonSpecies>` ajouté |

## Décisions

| Décision | Rationale | Impact |
|----------|-----------|--------|
| EggGroup1/2 = TEXT (pas FK) | Pas de table egg_groups dans le schéma Phase 1–5 ; scope simple Phase 6 | 06-02 FakemonAssembler lit les strings directement |
| Deux regex séparées dans SpriteValidator | ParseEntry utilise m.Groups[1/2/3] — une seule regex d'alternance collapse DexId+Identifier dans groupe 1 | SpriteEntry.DexId = null pour Fakemon ; comportement officiel préservé |
| Dragon Type1Id = 15 | Vérification SQLite : `SELECT id FROM pokemon_types WHERE identifier='dragon'` → 15 | Seeder sample fonctionnel dès Phase 6 |
| Config dans Configurations/ (pas Models/) | ApplyConfigurationsFromAssembly auto-scan ; dossier cohérent avec tous les configs existants | Aucune modification OnModelCreating requise |

## Déviations

| Type | Nb | Impact |
|------|----|--------|
| Correction chemin | 1 | Plan spécifiait `Models/`, réel = `Configurations/` — corrigé silencieusement |
| Erreur MSBuild transiente | 1 | MSB3492 cache stale sur SDK.Core ; build réel vert — bruit ignoré |

**Impact total :** corrections mineures, zéro scope creep.

## Issues

| Issue | Résolution |
|-------|-----------|
| MSB3492 `SDK.Core.AssemblyInfoInputs.cache` | Cache stale supprimé ; build clean ensuite |
| Chemin Plan `src/SDK.Data/Models/` inexistant | `src/SDK.Data/Configurations/` utilisé ; auto-scan non affecté |

## Next Phase Readiness

**Prêt :**
- `FakemonSpecies` entity + migration → 06-02 peut commencer FakemonAssemblyPipeline
- `FakemonDataSeeder` → test-dragon disponible en DB pour tests 06-02
- `FakemonPattern` → SpriteValidator gère déjà `fk_*.png`

**Concerns :**
- Tests unitaires `FakemonSpecies` déférés à 06-02 (prévu dans le plan)

**Blockers :** Aucun.

---
*Phase: 06-advanced-systems, Plan: 01*
*Complété: 2026-06-12*
