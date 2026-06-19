# Roadmap: PokemonSDK

## Overview

6 horizons : v0.1 (moteur core) → v0.2 (SDK NuGet) → v0.3 (CLI + docs) → v1.0 (moteur complet) → v1.x (plugin era) → v2.0 (réseau).

## Completed Milestones

**v0.1 Proof of Concept** (v0.1.0) — ✅ Complete 2026-06-05
Phases: 4 of 4 complete (Phases 1→4)

**v0.2 SDK Distribuable** (v0.2.0) — ✅ Complete 2026-06-07
Phases: 4 of 4 complete (Phase 5 ✅ — Phase 7 ✅ — Phase 8 ✅ — Phase 9 ✅)

## Next Milestone

**v0.3 CLI + Docs + Advanced Systems** (v0.3.0)
Status: Complete 2026-06-13
Phases: 3 of 3 complete (Phase 6 ✅ — Phase 10 ✅ — Phase 11 ✅)

## Future Milestones

**v1.0 Moteur Complet** (v1.0.0)
Status: In progress
Phases: 2 of 6 (Phase 12 ✅ — Phase 13 ✅ — Phase 14 — Phase 15 — Phase 16 — Phase 17)

**v1.x Plugin Era** (v1.1.0 → v1.4.0+)
Status: Not started
Phases: 0 of 4+ (Phase 18 v1.1 — Phase 19 v1.2 — Phase 20 v1.3 — Phase 21 v1.4 — futurs plugins v1.5+)

**v2.0 En ligne** (v2.0.0)
Status: Not started
Phases: 0 of 1 (Phase 22)

**v2.1 Double Battles** (v2.1.0)
Status: Not started
Phases: 0 of 1 (Phase 23)

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
| 9     | Sample Project          | 4     | ✅ Complete | 2026-06-07 |
| 6     | Advanced Systems        | 5     | ✅ Complete | 2026-06-12 |
| 10    | CLI pokeforge           | 3     | ✅ Complete | 2026-06-09 |
| 11    | Documentation           | 4     | ✅ Complete | 2026-06-13 |
| 12    | BattleScene UI          | 5     | ✅ Complete | 2026-06-14 |
| 13    | EXP + Level-up + Évol.  | 5     | ✅ Complete | 2026-06-17 |
| 14    | Items + Bag + Shop      | TBD   | Not started | -         |
| 15    | Party + PC + Pokédex UI | TBD   | Not started | -         |
| 16    | QuestPlugin             | TBD   | Not started | -         |
| 17    | Real Data Pipeline      | TBD   | Not started | -         |
| 18    | Audio complet           | TBD   | Not started | -         |
| 19    | Mécaniques modernes     | TBD   | Not started | -         |
| 20    | DungeonPlugin           | TBD   | Not started | -         |
| 21    | StreamerPlugin          | TBD   | Not started | -         |
| 22    | SDK.Network             | TBD   | Not started | -         |
| 23    | Double Battles 2v2      | TBD   | Not started | -         |

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

### Phase 6: Advanced Systems

**Goal:** TTS narration (INarrationPlugin + PiperNarrationPlugin + Lua binding sdk.tts.*) + Fakemon assembly pipeline (catalog→filter→assemble ImageSharp→export SQLite) + CLI `pokeforge fakemon list-parts/assemble`. ADV-03 + ADV-04.
**Depends on:** Phase 4 (Scripting), Phase 7 (SDK.Tools ImageSharp), Phase 10 (CLI infrastructure)
**Requirements:** ADV-03, ADV-04

**Plans:**

- [x] 06-01: FakemonSpecies entity + Migration AddFakemonSpecies + SpriteValidator regex D-16 D-23 ← *Done 2026-06-12*
- [x] 06-04: INarrationPlugin + SDK.Plugins.TTS (PiperNarrationPlugin — piper + aplay) ← *Done 2026-06-12*
- [x] 06-02: FakemonAssemblyPipeline (FakemonPartsCatalog + FakemonFilter + FakemonAssembler ImageSharp + FakemonExporter D-22) ← *Done 2026-06-12*
- [x] 06-03: pokeforge fakemon CLI (list-parts + assemble, InvocationContext pour 10 options) ← *Done 2026-06-12*
- [x] 06-05: TtsApi MoonSharp binding (sdk.tts.speak/stop/is_speaking) + SdkGlobals pattern + DoctorCommand TTS check ← *Done 2026-06-12*

---

### Phase 9: Sample Project

**Goal:** StarterGame NuGet-only (D-19) — BattleEngine 1v1 headless, NuzlockePlugin, Lua badge, ISaveSystem F5/F9. DX-04.
**Depends on:** Phase 8
**Requirements:** DX-04

**Plans:**

- [x] 09-01: PokeForge.SDK meta-package v0.1.0 (7 packages) ← *Done 2026-06-07*
- [x] 09-02: StarterGame scaffold NuGet-only consumer (D-19) ← *Done 2026-06-07*
- [x] 09-03: StarterGame Wave 2 — overworld CC0 Kenney + tilemap + joueur + BGM ← *Done 2026-06-07*
- [x] 09-04: StarterGame Wave 3 — SDK.Battle + Scripting + ISaveSystem intégrés ← *Done 2026-06-07*

---

### Phase 10: CLI pokeforge

**Goal:** `pokeforge new mon-jeu` → `dotnet run` en 30 secondes. CLI global tool `PokeForge.CLI` publié sur NuGet.org. Commandes : `new`, `asset-sync`, `seed`, `doctor`.
**Depends on:** Phase 9 (StarterGame stabilisé — D-20)
**Requirements:** DX-05

**Plans:**

- [x] 10-01: SDK.Cli (PackAsTool) + System.CommandLine + commande `new` + zip template StarterGame ← *Done 2026-06-09*
- [x] 10-02: `asset-sync` + `seed` (délèguent à SDK.Tools via ProjectReference) ← *Done 2026-06-09*
- [x] 10-03: `doctor` (checklist headless D-17) + publication NuGet tool + CI publish-cli.yml ← *Done 2026-06-09*

---

*Roadmap created: 2026-06-01 | Last updated: 2026-06-17 — Phase 13 EXP+LevelUp+Évolution ✅ Complete. v1.0: 2/6 phases done.*
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

### Phase 23: Double Battles 2v2

**Goal:** Combats 2v2 complets — DoubleBattleEngine parallele, ciblage par slot, moves de zone (spread ×0.75), DoubleBattleScene UI 4 HP bars.
**Depends on:** Phase 22 (architecture reseau stabilisee) — architecture PARALLELE, pas de modification de BattleState/IBattleEngine (D-26)
**Requirements:** DOUBLE-01

**Prep deja fait (Phase 13, 2026-06-17):**
- `BattleMode` enum dans SDK.Core/Enums (Single=0, Double=1)
- `BattleConfig.Mode = BattleMode.Single` champ additive — tous les appels existants non impactes

**Plans (TBD):**
- [ ] 23-01: DoubleBattleState + IDoubleBattleEngine + MoveTarget enum
- [ ] 23-02: DoubleBattleEngine (ciblage, spread ×0.75, ordre vitesse par slot)
- [ ] 23-03: DoubleBattleScene UI (4 HP bars, cible selection)
- [ ] 23-04: Tests + scenarios debug F6/F7
