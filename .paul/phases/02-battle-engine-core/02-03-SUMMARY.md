---
phase: 02-battle-engine-core
plan: 03
subsystem: battle-engine
tags: [battle, damage-formula, type-chart, difficulty-ai, immutable-state]

requires:
  - phase: 02-battle-engine-core plan 02-01
    provides: Move, Ability, Learnset, BattleConfig, BattleResult, DamageResult, IBattleEngine, IBattlePlugin, DifficultyMode enum
  - phase: 02-battle-engine-core plan 02-02
    provides: TypeEffectiveness (83 entrées Gen1), seeder battle data, 15 moves Gen1

provides:
  - BattleMove, BattlePokemon, BattleRequest, BattleState (sealed records immuables, D-05) dans SDK.Core
  - ITypeChart (interface abstraction du type chart) dans SDK.Core
  - IBattleEngine.RunBattle(BattleRequest) — signature corrigée
  - TypeChart (impl ITypeChart, dict in-memory depuis TypeEffectiveness)
  - IDamageFormula + Gen1DamageFormula (Gen=1) + StandardDamageFormula (Gen=4)
  - IDifficultyMode + StoryDifficultyMode (random) + HardDifficultyMode (max Power)
  - BattleEngine — combat 1v1 headless de start à KO, BattleState immuable + with

affects: [02-04, phase-3, phase-4, all-battle-consumers]

tech-stack:
  added: []
  patterns:
    - BattleState immuable sealed record + with expressions (D-05) — pattern de référence pour tous états combat
    - ITypeChart abstraction — TypeChart instancié depuis IEnumerable<TypeEffectiveness>, zéro couplage EF Core
    - IDamageFormula stratégie — switchable par génération sans modifier BattleEngine
    - IDifficultyMode stratégie — AI swappable à la construction (Story/Hard/futur Custom)

key-files:
  created:
    - src/SDK.Core/ValueObjects/BattleMove.cs
    - src/SDK.Core/ValueObjects/BattlePokemon.cs
    - src/SDK.Core/ValueObjects/BattleRequest.cs
    - src/SDK.Core/ValueObjects/BattleState.cs
    - src/SDK.Core/Interfaces/ITypeChart.cs
    - src/SDK.Battle/TypeChart.cs
    - src/SDK.Battle/Formulas/IDamageFormula.cs
    - src/SDK.Battle/Formulas/Gen1DamageFormula.cs
    - src/SDK.Battle/Formulas/StandardDamageFormula.cs
    - src/SDK.Battle/Difficulty/IDifficultyMode.cs
    - src/SDK.Battle/Difficulty/StoryDifficultyMode.cs
    - src/SDK.Battle/Difficulty/HardDifficultyMode.cs
    - src/SDK.Battle/BattleEngine.cs
  modified:
    - src/SDK.Core/Interfaces/IBattleEngine.cs

key-decisions:
  - "IBattleEngine.RunBattle(BattleRequest) — BattleConfig seul insuffisant, combattants nécessaires"
  - "ITypeChart dans SDK.Core — IBattleEngine y accède via interface, TypeChart impl dans SDK.Battle"
  - "Gen1 : SpecialAttack utilisée pour A ET D spéciaux — une seule stat Special en Gen 1"
  - "WeatherType.None comme état initial — Clear n'existe pas dans l'enum"

patterns-established:
  - "BattleEngine(IDamageFormula, IDifficultyMode×2, ITypeChart) — injection constructor, testable 02-04"
  - "ApplyMove privé retourne nouveau BattleState — jamais mutation en place (D-05)"
  - "TypeMultiplier = factor1 * factor2 (double type) — immunité à 0m court-circuite le calcul"

duration: ~20min
started: 2026-06-03T21:00:00Z
completed: 2026-06-03T21:20:00Z
---

# Phase 2 Plan 03 : SDK.Battle — BattleEngine Core

**Battle engine headless 1v1 complet : BattleState immuable (D-05), IDamageFormula Gen1+Standard, IDifficultyMode Story+Hard, BattleEngine RunBattle loop jusqu'à KO — 17/17 tests verts, zéro NuGet externe dans SDK.Battle.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~20 min |
| Démarré | 2026-06-03T21:00Z |
| Complété | 2026-06-03T21:20Z |
| Tâches | 3/3 complétées |
| Fichiers créés | 13 |
| Fichiers modifiés | 1 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : Types runtime SDK.Core + IBattleEngine signature | Pass | 5 types créés, IBattleEngine.RunBattle(BattleRequest), 4/4 Core tests verts |
| AC-2 : IDamageFormula × 2 + ITypeChart | Pass | Gen1.Generation==1, Standard.Generation==4, TypeChart.GetFactor défaut 1.0m |
| AC-3 : BattleEngine RunBattle headless | Pass | Compile, implémente IBattleEngine, 17/17 tests sans régression |

## Accomplishments

- **BattleState D-05** : sealed record immuable, `with` expressions dans ApplyMove — pattern de référence pour toute modification d'état combat
- **IDamageFormula × 2** : Gen1 (SpecialAttack bilatéral, crit 1/16, ×2) + Standard Gen4+ (SpAtk/SpDef distincts, crit 1/24, ×1.5)
- **TypeChart in-memory** : dict `(attackerType, defenderType, generation) → decimal`, défaut 1.0m si entrée absente — compatible avec les 83 entrées non-neutres du seeder
- **BattleEngine** : loop complet (max 200 tours), ordre Speed, accuracy check, STAB/double-type multiplier, KO detection, headless sans I/O

## Fichiers Créés / Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Core/ValueObjects/BattleMove.cs` | Créé | Move runtime avec PP courant, sealed record |
| `src/SDK.Core/ValueObjects/BattlePokemon.cs` | Créé | Pokémon en combat avec stats + type IDs, sealed record |
| `src/SDK.Core/ValueObjects/BattleRequest.cs` | Créé | Input RunBattle : Player + Opponent + Config |
| `src/SDK.Core/ValueObjects/BattleState.cs` | Créé | État immuable combat (D-05) : Player, Opponent, Turn, Weather, Config, Log |
| `src/SDK.Core/Interfaces/ITypeChart.cs` | Créé | GetFactor(attacker, defender, generation) → decimal |
| `src/SDK.Core/Interfaces/IBattleEngine.cs` | Modifié | RunBattle(BattleRequest) — signature corrigée |
| `src/SDK.Battle/TypeChart.cs` | Créé | ITypeChart impl in-memory depuis IEnumerable<TypeEffectiveness> |
| `src/SDK.Battle/Formulas/IDamageFormula.cs` | Créé | Interface : Generation + Calculate(attacker, defender, move, typeMultiplier, config) |
| `src/SDK.Battle/Formulas/Gen1DamageFormula.cs` | Créé | Formule Gen 1 — SpAtk bilatéral pour Special, crit ×2 (1/16) |
| `src/SDK.Battle/Formulas/StandardDamageFormula.cs` | Créé | Formule Gen 4+ — SpAtk/SpDef distincts, crit ×1.5 (1/24) |
| `src/SDK.Battle/Difficulty/IDifficultyMode.cs` | Créé | Interface : Mode + SelectMove(self, opponent, config) |
| `src/SDK.Battle/Difficulty/StoryDifficultyMode.cs` | Créé | IA random parmi moves PP>0 |
| `src/SDK.Battle/Difficulty/HardDifficultyMode.cs` | Créé | IA max Power parmi moves PP>0 |
| `src/SDK.Battle/BattleEngine.cs` | Créé | IBattleEngine impl — loop complet, ApplyMove immuable |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| IBattleEngine.RunBattle(BattleRequest) | BattleConfig seul n'incluait pas les combattants — interface incomplète en 02-01 | Déviation de 02-01 scope, correcte architecturalement |
| ITypeChart dans SDK.Core | IBattleEngine doit pouvoir y accéder — SDK.Core ne peut pas référencer SDK.Battle | Pattern injection propre, testable |
| Gen1 SpecialAttack pour A et D Special | Gen 1 avait une seule stat Special (pas de distinction SpAtk/SpDef) | Gen1DamageFormula fidèle à la mécanique originale |
| WeatherType.None (pas Clear) | Clear n'existe pas dans l'enum — None est la valeur neutre | Auto-fix mineur, aucun impact fonctionnel |

## Déviations du Plan

**1. IBattleEngine.RunBattle signature étendue**
- **Trouvé pendant :** Analyse pré-Task1
- **Problème :** Plan 02-01 avait défini `RunBattle(BattleConfig config)` — sans combattants, le battle engine ne peut pas fonctionner
- **Fix :** Ajout de BattleRequest (Player + Opponent + Config), modification IBattleEngine.cs
- **Impact :** Déviation de scope 02-01 documentée — amélioration architecturale nécessaire

**2. WeatherType.None au lieu de WeatherType.Clear**
- **Trouvé pendant :** Task 3 build (CS0117)
- **Problème :** L'enum WeatherType { None, Sun, Rain, Sand, Hail } — pas de valeur Clear
- **Fix :** Remplacement Clear → None dans BattleEngine.cs
- **Impact :** Aucun — None est sémantiquement correct (absence de météo)

## Readiness pour Plan 02-04

**Prêt :**
- `BattleEngine` constructeur injectable : `(IDamageFormula, IDifficultyMode, IDifficultyMode, ITypeChart)` — 100% testable
- `BattleState` immuable — tests peuvent inspecter l'état après chaque tour
- `Gen1DamageFormula` + `StandardDamageFormula` — formules testables indépendamment
- `TypeChart` instanciable depuis `List<TypeEffectiveness>` — pas besoin de DB dans les tests

**Déférés :**
- Switch Pokémon — déféré à 02-04 (tests) ou Phase 5 (plugins)
- Statuts (Sleep, Freeze, Burn, etc.) — D-11 s'applique quand statuts implémentés (Phase 5)
- Génération configurable dans BattleConfig (pour multi-gen) — 02-04 peut ajouter si besoin
- PP tracking réel dans le loop (les BattleMove ont CurrentPP mais non décrémenté) — 02-04

**Blockers :**
Aucun.

---
*Phase: 02-battle-engine-core, Plan: 03*
*Complété: 2026-06-03*
