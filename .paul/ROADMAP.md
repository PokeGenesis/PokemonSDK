# Roadmap: PokemonSDK

## Overview

3 horizons : moteur core jouable (v0.1) → SDK distribuable NuGet (v1.0) → CLI + docs + features avancées (v2.0).

## Completed Milestone

**v0.1 Proof of Concept** (v0.1.0) — ✅ Complete 2026-06-05
Phases: 4 of 4 complete (Phases 1→4)

## Current Milestone

**v1.0 SDK Distribuable** (v1.0.0)
Status: In progress
Phases: 3 of 4 complete (Phase 5 ✅ — Phase 7 ✅ — Phase 8 ✅ — Phase 9 restante)

## Phases

| Phase | Name          | Plans | Status      | Completed |
|-------|---------------|-------|-------------|-----------|
| 1     | SDK.Core + SDK.Data     | 4     | ✅ Complete | 2026-06-02 |
| 2     | Battle Engine Core      | 4     | ✅ Complete | 2026-06-04 |
| 3     | World Foundation        | 4     | ✅ Complete | 2026-06-05 |
| 4     | Scripting + Progression | 3     | ✅ Complete | 2026-06-05 |
| 5     | Plugins + Characters    | 3     | ✅ Complete | 2026-06-06 |
| 7     | Developer Experience    | 4     | ✅ Complete | 2026-06-06 |
| 8     | NuGet Distribution      | 4     | ✅ Complete | 2026-06-07 |
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

- [x] 03-01: Migration 003 (encounter_zones) + SDK.Core world primitives ← *Done 2026-06-04*
- [x] 03-02: EncounterSystem + RealTimeClock + WeatherSystem + tests ← *Done 2026-06-04*
- [x] 03-03: Game1 + WorldSystem + PlayerSystem + RenderPipeline (xBR) + day/night ← *Done 2026-06-05*
- [x] 03-04: HeadlessSmokeTester + CI GitHub Actions matrix (ubuntu + windows) ← *Done 2026-06-05*

### Phase 4: Scripting + Progression

**Goal:** Script Lua sur NPC modifie GameState, badge attribué, save/load restaure état complet.
**Depends on:** Phase 3
**Research:** Unlikely (MoonSharp validé ancienne base)
**Requirements:** SCRIPT-01→03

**Plans:**

- [x] 04-01: SDK.Scripting (IScriptEngine + LuaScriptEngine SoftSandbox + GameState + coroutines) ← *Done 2026-06-05*
- [x] 04-02: Migration 004 (trainers/badges) + Lua badge/flag API + OnNpcInteraction ← *Done 2026-06-05*
- [x] 04-03: ISaveSystem + SaveSystem JSON + DialogueBox + scripts prod + Game1 wiring ← *Done 2026-06-05*

### Phase 5: Plugins + Characters

**Goal:** 3 plugins IBattlePlugin (Nuzlocke, Randomizer, Turbo) + entités Character/VillainGroup/VillainMember avec D-22 multilingue.
**Depends on:** Phase 4
**Requirements:** BATTLE-04→06, CHAR-01

**Plans:**

- [x] 05-01: IPlugin base + multi-surface PluginRegistry + IBattlePlugin interface ← *Done 2026-06-06*
- [x] 05-02: NuzlockePlugin + RandomizerPlugin + TurboPlugin (src/plugins/) ← *Done 2026-06-06*
- [x] 05-03: Character + VillainGroup + VillainMember + Migration AddCharacterData + D-22 seeding ← *Done 2026-06-06*

---

*Roadmap created: 2026-06-01 | Last updated: 2026-06-07 — Phase 8 NuGet Distribution Complete*
*Full details: ROADMAP.md (root)*

### Phase 7: Developer Experience

**Goal:** Asset pipeline automatique (SpriteValidator + AtlasPacker + SqliteSyncer) + hot reload Lua (<500ms) + LuaConsole REPL ingame. DX-01 + DX-02.
**Depends on:** Phase 5
**Requirements:** DX-01, DX-02

**Plans:**

- [x] 07-01: SpriteValidator + SpriteScanner + CLI asset-validate ← *Done 2026-06-06*
- [x] 07-02: AtlasPacker + SqliteSyncer + CLI asset-sync + import.json ← *Done 2026-06-06*
- [x] 07-03: LuaHotReloader (#if DEBUG) + LuaErrorOverlay + IScriptEngine.Reload ← *Done 2026-06-06*
- [x] 07-04: LuaConsole REPL (toggle ~) + MGCB DefaultFont.xnb + Draw() réels ← *Done 2026-06-06*

### Phase 8: NuGet Distribution

**Goal:** 7 packages PokeForge.SDK.* publiables sur NuGet.org v0.1.0 — métadonnées, licences, CI/CD, smoke test consumer.
**Depends on:** Phase 7
**Requirements:** DX-03

**Plans:**

- [x] 08-01: NuGet metadata (PackageId, Description, Authors, Icon, README, RepositoryUrl) ← *Done 2026-06-07*
- [x] 08-02: Licence/CVE cleanup — SixLabors.ImageSharp 4.0.0 + sixlabors.lic + FA v8 open-source ← *Done 2026-06-07*
- [x] 08-03: publish-nuget.yml CI/CD — pack + push 7 packages sur NuGet.org (secrets NUGET_API_KEY + SIXLABORS_LICENSE_KEY) ← *Done 2026-06-07*
- [x] 08-04: NuGetConsumerSmokeTest — 7 typeof() verts depuis feed local, D-19 validé ← *Done 2026-06-07*
