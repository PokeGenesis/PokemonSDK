---
phase: 13-exp-levelup-evolution
plan: 01
subsystem: battle
tags: [exp, levelup, growthrate, formula, migration, plugins]

requires:
  - phase: 12-battlescene-ui
    provides: BattleScene foundation, IBattlePlugin hooks including OnPokemonLevelUp

provides:
  - GrowthRate enum + IExpFormula interface (SDK.Core, zero NuGet)
  - Gen1ExpFormula + Gen5ExpFormula implementations
  - BattlePokemon: CurrentExp, BaseExpYield, GrowthRate, FullLearnset (backward-compat)
  - PokemonSpecies: BaseExpYield + GrowthRate + EF Core migration AddExpSystem
  - BattleEngine: EXP award + level-up loop + stat scaling + PluginRegistry.NotifyLevelUp
  - 17 new tests (11 ExpFormulaTests + 6 BattleEngineExpTests)

affects: [13-02-expbar-ui, 13-03-evolution-scene, 17-real-data-pipeline]

tech-stack:
  added: []
  patterns:
    - "IExpFormula: strategy pattern for swappable Gen1/Gen5 EXP formulas"
    - "BattlePokemon FullLearnset: IReadOnlyList<(int Level, BattleMove Move)>? — constructed at call site, no DB query in BattleEngine"
    - "EF migration startup-project: always src/SDK.Data (not SDK.MonoGame — lacks EF Design)"

key-files:
  created:
    - src/SDK.Core/Enums/GrowthRate.cs
    - src/SDK.Core/Interfaces/IExpFormula.cs
    - src/SDK.Battle/Formulas/Gen1ExpFormula.cs
    - src/SDK.Battle/Formulas/Gen5ExpFormula.cs
    - src/SDK.Data/Migrations/20260614132733_AddExpSystem.cs
    - tests/SDK.Battle.Tests/ExpFormulaTests.cs
    - tests/SDK.Battle.Tests/BattleEngineExpTests.cs
  modified:
    - src/SDK.Core/Entities/PokemonSpecies.cs
    - src/SDK.Core/ValueObjects/BattlePokemon.cs
    - src/SDK.Data/Configurations/PokemonSpeciesConfiguration.cs
    - src/SDK.Battle/BattleEngine.cs
    - src/SDK.Battle/Plugins/PluginRegistry.cs

key-decisions:
  - "Stat scaling = (int)(oldStat * (newLevel+5.0)/(oldLevel+5.0)) — heuristic approché, pas IVs/EVs (hors scope)"
  - "trainerBattle=false hardcodé dans BattleEngine — distinction trainer/wild déférée Phase 14"
  - "Level cap = 100 dans la boucle AwardExp"
  - "GrowthRate source de vérité = PokemonSpecies.GrowthRate (DB) — BattlePokemon.GrowthRate est une copie fournie à la construction (factory Phase 15/17)"

patterns-established:
  - "IExpFormula dans SDK.Core : swappable Gen1/Gen5, configurable par génération"
  - "AwardExp() privé dans BattleEngine : EXP + multi-level-up + learnset log + NotifyLevelUp"
  - "BattlePokemon params optionnels en fin de record : backward-compat sans migration call sites"

duration: ~10min
started: 2026-06-14T13:25:00Z
completed: 2026-06-14T13:33:00Z
---

# Phase 13 Plan 01: EXP + Level-Up Headless Engine

**GrowthRate/IExpFormula dans SDK.Core (zero NuGet), Gen1/Gen5 formulas, BattlePokemon étendu, migration AddExpSystem, BattleEngine award EXP + level-up loop après KO ennemi — 64/64 tests verts, commit 02b20e2.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~10 min |
| Démarré | 2026-06-14 ~15:25 CEST |
| Complété | 2026-06-14 ~15:33 CEST |
| Tâches | 14/14 complètes |
| Fichiers modifiés | 15 (7 créés, 5 modifiés, 1 migration + snapshot) |
| Commit | `02b20e2` |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: GrowthRate + IExpFormula dans SDK.Core (zéro NuGet) | PASS | `dotnet list package` — vide. 4 valeurs enum. |
| AC-2: BattlePokemon backward-compat 4 params default | PASS | Tous call sites existants compilent sans modification. |
| AC-3: PokemonSpecies + migration AddExpSystem | PASS | NOT NULL, default 64/0, Down() réversible. |
| AC-4: Gen1/Gen5 formulas correctes | PASS | Gen1: 228, 342, 714. Seuils: MediumFast=1000, Slow=1250. |
| AC-5: BattleEngine EXP + level-up après KO | PASS | Loop multi-level, stat scaling, learnset log. |
| AC-6: Backward-compat sans IExpFormula | PASS | `expFormula=null` par défaut — 0 régression tests existants. |
| AC-7: PluginRegistry.NotifyLevelUp | PASS | Dispatche `OnPokemonLevelUp(pokemon, oldLevel, newLevel)` à tous les plugins. |
| AC-8: 17 nouveaux tests verts, 0 régression | PASS | 64/64 SDK.Battle.Tests. 230+ total toutes assemblées. |

## Accomplissements

- `IExpFormula` dans SDK.Core: contrat swappable Gen1/Gen5 sans NuGet (D-01 maintenu)
- `BattleEngine.AwardExp()`: EXP gain, boucle level-up cap 100, scaling stats proportionnel, log moves appris via FullLearnset
- Migration EF Core `AddExpSystem` réversible — `PokemonSpecies` a maintenant `BaseExpYield` (default 64) et `GrowthRate` (default MediumFast=0) en DB
- `PluginRegistry.NotifyLevelUp` câble les plugins existants (NuzlockePlugin, etc.) au level-up sans modification de IBattlePlugin

## Task Commits

Plan 13-01 committé atomiquement en un seul commit :

| Commit | Description |
|--------|-------------|
| `02b20e2` | feat(exp): Phase 13-01 — EXP system headless engine (15 files, 1828 insertions) |

## Fichiers Créés/Modifiés

| Fichier | Action | Contenu |
|---------|--------|---------|
| `src/SDK.Core/Enums/GrowthRate.cs` | Créé | Enum 4 valeurs : MediumFast=0, MediumSlow=1, Fast=2, Slow=3 |
| `src/SDK.Core/Interfaces/IExpFormula.cs` | Créé | CalcExpGain + ExpThreshold — zero NuGet |
| `src/SDK.Core/Entities/PokemonSpecies.cs` | Modifié | +BaseExpYield=64, +GrowthRate=MediumFast |
| `src/SDK.Core/ValueObjects/BattlePokemon.cs` | Modifié | +CurrentExp=0, +BaseExpYield=64, +GrowthRate=MediumFast, +FullLearnset=null |
| `src/SDK.Data/Configurations/PokemonSpeciesConfiguration.cs` | Modifié | +HasDefaultValue(64) + HasConversion<int>() pour GrowthRate |
| `src/SDK.Data/Migrations/20260614132733_AddExpSystem.cs` | Créé | AddColumn BaseExpYield + GrowthRate, Down() DropColumn |
| `src/SDK.Battle/Formulas/Gen1ExpFormula.cs` | Créé | `(int)(mult * baseYield * opponentLevel / 7.0)` + 4 GrowthRate seuils |
| `src/SDK.Battle/Formulas/Gen5ExpFormula.cs` | Créé | `(int)(Math.Pow(baseYield * opponentLevel, 1/2.5) * mult / 5 + 2)` + mêmes seuils |
| `src/SDK.Battle/BattleEngine.cs` | Modifié | +IExpFormula? param, +AwardExp() privé, intégration dans RunTurn après KO |
| `src/SDK.Battle/Plugins/PluginRegistry.cs` | Modifié | +NotifyLevelUp(pokemon, oldLevel, newLevel) |
| `tests/SDK.Battle.Tests/ExpFormulaTests.cs` | Créé | 11 tests Gen1+Gen5 CalcExpGain+ExpThreshold+monotonicity |
| `tests/SDK.Battle.Tests/BattleEngineExpTests.cs` | Créé | 6 tests intégration EXP/level-up/plugin/backward-compat/stat-scaling |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| Stat scaling = `(int)(old * (newLvl+5)/(oldLvl+5))` | Heuristique simple, pas IVs/EVs/nature (hors scope Phase 13) | Stats cohérentes sans complexité gen-specific |
| `trainerBattle=false` hardcodé | Distinction trainer/wild = Phase 14 (bag, items) | API prête, logique reportée |
| `FullLearnset` construit au call site | Évite DB query dans BattleEngine — D-06 et perf | Consommateur passe la liste, BattleEngine reste pur |
| Commit unique (pas atomique par tâche) | Plan atomique court, pas de rollback partiel utile | OK pour plan headless sans checkpoint |

## Déviations du Plan

| Type | Count | Impact |
|------|-------|--------|
| Auto-fixed | 3 | Essentiel, zéro scope creep |
| Déféré | 0 | Néant |

**Auto-fixed :**

**1. MSB3492 stale cache (E-05 connu)**
- Trouvé lors de : Task 1 (build SDK.Core)
- Problème : `-q` affichait "1 Error(s)" mais build réel propre
- Fix : `rm obj/Debug/net10.0/SDK.Core.csproj.CoreCompileInputs.cache`
- Vérification : rebuild 0 erreurs

**2. DamageResult constructeur 3 paramètres**
- Trouvé lors de : Task 12 (BattleEngineExpTests)
- Problème : `new DamageResult(damage, false)` → CS7036 (TypeMultiplier manquant)
- Fix : `new DamageResult(damage, false, 1.0m)` dans le mock setup
- Vérification : test compile + passe

**3. using SDK.Battle.Difficulty manquant dans test**
- Trouvé lors de : Task 12
- Problème : CS0246 `IDifficultyMode` non trouvé
- Fix : ajout `using SDK.Battle.Difficulty;`
- Vérification : compilation propre

## Next Phase Readiness

**Prêt pour Plan 13-02 (ExpBar UI + LevelUpOverlay) :**
- `IExpFormula` swappable — SDK.MonoGame peut injecter Gen1 ou Gen5
- `BattleState.Log` contient "gained X EXP!" + "grew to level N!" — consommables par l'UI
- `PluginRegistry.NotifyLevelUp` câblé — AnimationPlugin (Plan 13-02) peut s'y accrocher
- Migration `AddExpSystem` — DB prête pour données réelles (Phase 17)

**Concerns :**
- `trainerBattle=false` hardcodé — à corriger quand BattleEngine distingue trainer/wild (Phase 14)
- Stat scaling heuristique — exact uniquement si IVs/EVs/nature = 0 (acceptable jusqu'à Phase 17)

**Blockers :** Aucun.

---
*Phase: 13-exp-levelup-evolution, Plan: 01*
*Complété: 2026-06-14*
