# Roadmap: PokemonSDK

## Overview

3 horizons : moteur core jouable (v0.1) → SDK distribuable NuGet (v1.0) → CLI + docs + features avancées (v2.0).

## Current Milestone

**v0.1 Proof of Concept** (v0.1.0)
Status: In progress
Phases: 2 of 4 complete

## Phases

| Phase | Name          | Plans | Status      | Completed |
|-------|---------------|-------|-------------|-----------|
| 1     | SDK.Core + SDK.Data     | 4     | ✅ Complete | 2026-06-02 |
| 2     | Battle Engine Core      | 4     | ✅ Complete | 2026-06-04 |
| 3     | World Foundation        | 4     | Planning    | -         |
| 4     | Scripting + Progression | 3     | Not started | -         |
| 5     | Plugins + Characters    | TBD   | Not started | -         |
| 7     | Developer Experience    | 4     | Not started | -         |
| 8     | NuGet Distribution      | 4     | Not started | -         |
| 9     | Sample Project          | 4     | Not started | -         |
| 6     | Advanced Systems        | TBD   | Not started | -         |
| 10    | CLI pokeforge           | 4     | Not started | -         |
| 11    | Documentation           | 4     | Not started | -         |

## Phase Details

### Phase 1: SDK.Core + SDK.Data

**Goal:** Un développeur peut créer un Pokémon, le persister en SQLite, le requêter avec filtre génération, lire son nom en 6 langues. SDK.Core sans aucune dépendance externe.
**Depends on:** Nothing (first phase)
**Research:** Unlikely (patterns EF Core connus, validés sur .NET 8)
**Requirements:** DATA-01→06, PLAT-01, PLAT-03

**Plans:**

- [x] 01-01: Solution scaffold .NET 10 + SDK.Core domain models + CoreDependencyTests ← *Done 2026-06-01*
- [x] 01-02: EF Core 10 + PokemonDbContext + Fluent API + Migration 001 + SqliteTestFixture ← *Done 2026-06-02*
- [x] 01-03: DbContextExtensions (GetByGeneration/GetTranslations) + DataSeeder + seed CLI ← *Done 2026-06-02*
- [x] 01-04: End-to-end SDK test + PLAT-01/03 cross-target scan ← *Done 2026-06-02*

### Phase 2: Battle Engine Core

**Goal:** Combat 1v1 headless de start à KO avec données DB réelles, IA configurable, formules par génération.
**Depends on:** Phase 1
**Research:** Unlikely (BattleState immuable, formules validés ancienne base)
**Requirements:** BATTLE-01→03, BATTLE-07

**Plans:**

- [x] 02-01: SDK.Core battle models (Move, Learnset, Ability, TypeEffectiveness, BattleConfig, enums) ← *Done 2026-06-03*
- [x] 02-02: SDK.Data Migration 002 (types, moves, type chart 18×18, abilities) + BattleDataSeeder ← *Done 2026-06-03*
- [x] 02-03: SDK.Battle (BattleState immuable, IDamageFormula ×2, IDifficultyMode ×2, BattleEngine) ← *Done 2026-06-03*
- [x] 02-04: SDK.Battle.Tests (loop, damage, AI, config, STAB, type immunity) ← *Done 2026-06-04*

### Phase 3: World Foundation

**Goal:** Joueur sur tilemap MonoGame, déplacement avec collision, rencontre sauvage → battle engine.
**Depends on:** Phase 2
**Research:** Likely (MonoGame.Extended.Tiled, xBR shader integration)
**Requirements:** MAP-01→03, PLAT-02

**Plans:**

- [ ] 03-01: Migration 003 (encounter_zones) + SDK.Core world primitives
- [ ] 03-02: EncounterSystem + RealTimeClock + WeatherSystem + tests
- [ ] 03-03: Game1 + WorldSystem + PlayerSystem + RenderPipeline (xBR) + day/night
- [ ] 03-04: HeadlessSmokeTester + CI GitHub Actions matrix (ubuntu + windows)

### Phase 4: Scripting + Progression

**Goal:** Script Lua sur NPC modifie GameState, badge attribué, save/load restaure état complet.
**Depends on:** Phase 3
**Research:** Unlikely (MoonSharp validé ancienne base)
**Requirements:** SCRIPT-01→03

**Plans:**

- [ ] 04-01: SDK.Scripting (IScriptEngine + LuaScriptEngine SoftSandbox + GameState + coroutines)
- [ ] 04-02: Migration 004 (trainers/badges) + Lua badge/flag API + OnNpcInteraction
- [ ] 04-03: ISaveSystem + SaveSystem JSON + DialogueBox + scripts prod + Game1 wiring

---

*Roadmap created: 2026-06-01*
*Full details: ROADMAP.md (root)*
