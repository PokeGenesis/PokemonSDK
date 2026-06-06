---
phase: 05-plugins-characters
plan: 03
subsystem: database
tags: [efcore, sqlite, translations, d22, characters, villain, seeding]

requires:
  - phase: 05-02
    provides: IPlugin base + multi-surface PluginRegistry + NuzlockePlugin + RandomizerPlugin + TurboPlugin

provides:
  - Character, VillainGroup, VillainMember entities in SDK.Core.Entities
  - EF Core migration AddCharacterData (tables characters, villain_groups, villain_members)
  - CharacterDataSeeder: 5 Characters × 6 locales + 1 VillainGroup × 6 locales + 2 VillainMembers
  - D-22 Characters deferred issue resolved
  - TurboPlugin.TextSpeedMultiplier property for all dialogue types

affects: [Phase 7 DX, Phase 8 NuGet, Phase 9 Sample]

tech-stack:
  added: []
  patterns:
    - "CharacterDataSeeder: même pattern tuple (id, en, es, fr, de, it, ja) + nested foreach que ProgressionDataSeeder"
    - "VillainMemberConfiguration minimal: table + HasKey seulement, FK via CharacterConfiguration/VillainGroupConfiguration"
    - "TurboPlugin: property float exposée pour renderers MonoGame, pas de logique interne"

key-files:
  created:
    - src/SDK.Core/Entities/Character.cs
    - src/SDK.Core/Entities/VillainGroup.cs
    - src/SDK.Core/Entities/VillainMember.cs
    - src/SDK.Data/Configurations/CharacterConfiguration.cs
    - src/SDK.Data/Configurations/VillainGroupConfiguration.cs
    - src/SDK.Data/Configurations/VillainMemberConfiguration.cs
    - src/SDK.Data/Seeding/CharacterDataSeeder.cs
    - src/SDK.Data/Migrations/20260606133108_AddCharacterData.cs
    - tests/SDK.Data.Tests/CharacterDataTests.cs
  modified:
    - src/SDK.Data/PokemonDbContext.cs
    - src/SDK.Data/Seeding/DataSeeder.cs
    - src/plugins/SDK.Plugins.Turbo/TurboPlugin.cs

key-decisions:
  - "TurboPlugin.TextSpeedMultiplier = float.MaxValue par défaut; forcé à 1.0f si IsActive=false — renderers doivent lire les deux propriétés"
  - "VillainMemberConfiguration: table + HasKey seulement — FK déjà déclarés dans CharacterConfiguration et VillainGroupConfiguration"
  - "Character vs Trainer: entités séparées — Trainer = gym leaders (progression), Character = narrative (rivaux, antagonistes)"
  - "EntityType = 'Character' / 'VillainGroup' PascalCase — cohérent avec Badge, Move, Ability"

patterns-established:
  - "Guard D-22 par EntityType: if (ctx.Translations.Any(t => t.EntityType == 'X')) return;"
  - "Two-context test pattern SqliteTestFixture: ctx1 seed, ctx2 assert (évite EF change tracker interference)"

duration: ~30min
started: 2026-06-06T13:28:00Z
completed: 2026-06-06T14:10:00Z
---

# Phase 5 Plan 03: Characters + D-22 Summary

**Character, VillainGroup, VillainMember ajoutés en SDK.Core avec migration EF Core AddCharacterData, seeding D-22 complet (36 translations) et 10 nouveaux tests — deferred issue D-22 Characters résolu.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~30 min |
| Started | 2026-06-06T13:28:00Z |
| Completed | 2026-06-06T14:10:00Z |
| Tasks | 3 complétées |
| Files modified | 12 (9 créés, 3 modifiés) |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: SDK.Core sans NuGet (D-01) | Pass | `dotnet list SDK.Core.csproj package` → vide |
| AC-2: Migration AddCharacterData appliquée | Pass | Tables characters, villain_groups, villain_members créées |
| AC-3: CharacterTranslationsD22Tests 3/3 verts | Pass | D-22 résolu: 5×6=30 rows Character + 1×6=6 rows VillainGroup |
| AC-4: ≥125 tests, 0 failed | Pass | 126 tests, 0 failed (116 existants + 10 nouveaux) |

## Accomplishments

- 3 entités SDK.Core.Entities: Character (Id, Identifier, Role, Generation, VillainMemberships), VillainGroup (Id, Identifier, Generation, Members), VillainMember (Id, CharacterId, VillainGroupId + navigations)
- Migration `20260606133108_AddCharacterData` générée et appliquée — 3 tables snake_case
- CharacterDataSeeder: 5 Characters + 30 translations + 1 VillainGroup + 6 translations + 2 VillainMembers, entièrement idempotent
- TurboPlugin enrichi: `TextSpeedMultiplier` (float.MaxValue = instant, 1.0f = normal) couvre tous dialogues (battle + NPC + menus)
- 10 nouveaux tests: CharacterTranslationsD22Tests (3), CharacterSeederIntegrationTests (4), CharacterCrudTests (3)

## Task Results

| Task | Status | Notes |
|------|--------|-------|
| T0: Entités Character/VillainGroup/VillainMember | PASS | 3 fichiers créés, D-01 intact |
| T1: Configs EF + DbContext + Migration + Seeder | PASS | Migration appliquée, DataSeeder.SeedAll() branché |
| T2: Tests CharacterTranslationsD22Tests + CRUD | PASS | 10 tests créés, plan demandait ≥6 |

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Core/Entities/Character.cs` | Created | Entité narrative (rivaux, antagonistes, champions) |
| `src/SDK.Core/Entities/VillainGroup.cs` | Created | Groupe de méchants (ex: Team Rocket) |
| `src/SDK.Core/Entities/VillainMember.cs` | Created | Lien M:M Character ↔ VillainGroup |
| `src/SDK.Data/Configurations/CharacterConfiguration.cs` | Created | Fluent API: table characters, index unique Identifier, FK Members |
| `src/SDK.Data/Configurations/VillainGroupConfiguration.cs` | Created | Fluent API: table villain_groups, index unique Identifier, FK Members |
| `src/SDK.Data/Configurations/VillainMemberConfiguration.cs` | Created | Fluent API minimal: table villain_members, HasKey seulement |
| `src/SDK.Data/PokemonDbContext.cs` | Modified | +3 DbSets: Characters, VillainGroups, VillainMembers |
| `src/SDK.Data/Seeding/CharacterDataSeeder.cs` | Created | SeedAll(): 5 chars + 1 group + 2 members + 36 translations D-22 |
| `src/SDK.Data/Seeding/DataSeeder.cs` | Modified | +CharacterDataSeeder.SeedAll(ctx) après ProgressionDataSeeder |
| `src/SDK.Data/Migrations/20260606133108_AddCharacterData.cs` | Created | Migration EF Core auto-générée |
| `src/plugins/SDK.Plugins.Turbo/TurboPlugin.cs` | Modified | +TextSpeedMultiplier float property |
| `tests/SDK.Data.Tests/CharacterDataTests.cs` | Created | 10 tests D-22 + intégration + CRUD |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| TurboPlugin.TextSpeedMultiplier = float | Renderers MonoGame doivent pouvoir lire la vitesse texte sans logique interne au plugin | Renderers lisent IsActive + TextSpeedMultiplier — contrat clair |
| VillainMemberConfiguration minimal | FK déjà déclarés dans CharacterConfig et VillainGroupConfig via HasMany/WithOne — re-déclarer causerait erreur EF | Configuration plus propre, pas de double-registration |
| Character.Role = string libre | Évite breaking change si nouveaux rôles émergent (Sage, Elder, etc.) | Pas d'enum dans SDK.Core — plus flexible |

## Deviations from Plan

### Summary

| Type | Count | Impact |
|------|-------|--------|
| Auto-fixed | 1 | Fix typo plan (James DE = "James" pas "Jessie") |
| Scope additions | 1 | 10 tests créés au lieu de ≥6 planifiés (CharacterSeederIntegrationTests ajouté pour atteindre AC-4 ≥125) |
| Deferred | 0 | Aucun |

### Auto-fixed Issues

**1. Typo dans plan — James traduction DE**
- **Found during:** T1 (SeedCharacterTranslations)
- **Issue:** Tuple plan ligne James: `"James", "James", "James", "Jessie", "James", "コジロウ"` — DE = "Jessie" (copy-paste)
- **Fix:** Corrigé en `"James"` dans CharacterDataSeeder.cs
- **Verification:** CharacterTranslationsD22Tests passe — 6 locales correctes pour chaque Character

## Issues Encountered

| Issue | Resolution |
|-------|------------|
| MSBuild tail-N output: `tail -3` montrait "4 Error(s)" phantom | Grep sur sortie complète montrait "0 Error(s)" — tail-N capture sous-totaux MSBuild par projet, pas le résumé final |
| Test count shortfall (6 tests → 122 < 125 cible) | Ajout classe CharacterSeederIntegrationTests (4 tests idempotency/counts) → 10 tests, 126 total |

## Next Phase Readiness

**Ready:**
- Character/VillainGroup infrastructure prête pour Phase 6 (interactions Lua, dialogue, GameState Character)
- TurboPlugin.TextSpeedMultiplier exposé — Phase 7 DX peut le connecter au renderer
- 126 tests verts, 0 dette technique ajoutée

**Concerns:**
- TilemapRenderer stub (Phase 3) toujours absent — Phase 7 DX le résoudra
- CHAR-02 (rivaux avec stats propres) et CHAR-03 (antagonistes avec arcs narratifs) partiellement couverts — entités créées mais aucune logique narrative encore

**Blockers:**
- None

---
*Phase: 05-plugins-characters, Plan: 03*
*Completed: 2026-06-06*
