---
phase: 05-plugins-characters
plan: 02
subsystem: plugins
tags: [IPlugin, IBattlePlugin, PluginRegistry, NuzlockePlugin, RandomizerPlugin, TurboPlugin, multi-surface]

requires:
  - phase: 05-01
    provides: IBattlePlugin contrat, PluginRegistry v1, BattleEngine hooks, 108 tests verts

provides:
  - IPlugin base interface dans SDK.Core — contrat racine de tout plugin SDK
  - IBattlePlugin : IPlugin — backward-compat (Name hérité, 9 hooks inchangés)
  - PluginRegistry multi-surface — Register(IPlugin), dispatch OfType<T>()
  - NuzlockePlugin — mort permanente via callback Action<string, bool>
  - RandomizerPlugin — RandomizePokemon() déterministe par seed
  - TurboPlugin — marqueur IsActive pour renderers MonoGame
  - PluginRegistry enregistré en DI dans Program.cs
  - Convention src/plugins/ établie — répertoire dédié tous SDK.Plugins.*

affects: [05-03-characters, 05-04-adv, phase-6-encounter, phase-7-map, phase-8-nuget]

tech-stack:
  added: []
  patterns:
    - "Multi-surface plugin dispatch via OfType<T>() dans PluginRegistry"
    - "Plugin src dans src/plugins/SDK.Plugins.{Nom}/, tests dans tests/SDK.Plugins.{Nom}.Tests/"
    - "Plugin callback (NuzlockePlugin) — mutation état game déléguée à l'appelant"
    - "Plugin déterministe (RandomizerPlugin) — seed fixé au constructeur, _rng stateful"
    - "Plugin marqueur (TurboPlugin) — propriété IsActive, hooks no-op"

key-files:
  created:
    - src/SDK.Core/Interfaces/IPlugin.cs
    - src/plugins/SDK.Plugins.Nuzlocke/NuzlockePlugin.cs
    - src/plugins/SDK.Plugins.Randomizer/RandomizerPlugin.cs
    - src/plugins/SDK.Plugins.Turbo/TurboPlugin.cs
    - tests/SDK.Plugins.Nuzlocke.Tests/NuzlockePluginTests.cs
    - tests/SDK.Plugins.Nuzlocke.Tests/AllPluginsSmokeTests.cs
    - tests/SDK.Plugins.Randomizer.Tests/RandomizerPluginTests.cs
  modified:
    - src/SDK.Core/Interfaces/IBattlePlugin.cs
    - src/SDK.Battle/Plugins/PluginRegistry.cs
    - src/SDK.MonoGame/Program.cs
    - PokemonSDK.slnx

key-decisions:
  - "IPlugin.Name = seul membre — base minimaliste extensible par sous-interfaces par domaine"
  - "src/plugins/ sous-dossier physique dédié — convention définitive pour tous SDK.Plugins.*"
  - "AllPluginsSmokeTests dans SDK.Plugins.Nuzlocke.Tests — évite un 3e projet test pour Turbo seul"
  - "SDK.MonoGame.csproj : zéro ref SDK.Plugins.* — PluginRegistry accessible via SDK.Battle transitif"

patterns-established:
  - "Nouveau plugin : src/plugins/SDK.Plugins.{Nom}/ + refs ../../SDK.Core + ../../SDK.Battle"
  - "Test plugin : tests/SDK.Plugins.{Nom}.Tests/ + refs ../../src/plugins/SDK.Plugins.{Nom}/"
  - "MakePokemon helper inline dans chaque projet test — BattleTestHelpers est internal à SDK.Battle.Tests"

duration: ~90min
started: 2026-06-06T12:28:00Z
completed: 2026-06-06T16:10:00Z
---

# Phase 5 Plan 02: Plugin Architecture Multi-Surface Summary

**Architecture plugin game-wide posée : IPlugin base + PluginRegistry multi-surface + 3 plugins concrets (Nuzlocke, Randomizer, Turbo) + convention src/plugins/ établie — 116 tests verts, 0 régression.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~90 min |
| Démarré | 2026-06-06T12:28:00Z |
| Complété | 2026-06-06T16:10:00Z |
| Tâches | 4 (T0→T3) + post-APPLY refactor structure |
| Fichiers créés | 11 |
| Fichiers modifiés | 4 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-0: IPlugin base + refactor backward-compat | Pass | 108 tests 05-01 verts sans modification |
| AC-1: NuzlockePlugin callback key `nuzlocke_dead_{id}` | Pass | 3 tests NuzlockePluginTests |
| AC-2: NuzlockePlugin intégration BattleEngine | Pass | Callback déclenché sur KO |
| AC-3: RandomizerPlugin déterminisme seed + range [1,18] | Pass | 3 tests RandomizerPluginTests |
| AC-4: TurboPlugin IsActive + smoke 3 plugins | Pass | 2 tests AllPluginsSmokeTests |
| AC-5: Build 0 erreurs + ≥ 116 tests verts | Pass | 116 tests, 0 failed, 0 warnings |

## Accomplissements

- **IPlugin base** dans `SDK.Core.Interfaces` — contrat racine minimal (`string Name`) pour toute la hiérarchie plugin v1.0 (IBattlePlugin → IEncounterPlugin Phase 6 → IMapPlugin Phase 7)
- **PluginRegistry multi-surface** — `List<IPlugin>` + dispatch `OfType<IBattlePlugin>()` — accepte tout plugin, exécute seulement les sous-interfaces pertinentes par domaine
- **3 plugins concrets** D-13 — NuzlockePlugin (callback mort permanente), RandomizerPlugin (seed déterministe), TurboPlugin (marqueur render-speed) — jamais des modes hardcodés
- **Convention `src/plugins/`** établie et documentée en STATE.md — répertoire physique dédié, zéro friction pour les prochains plugins

## Files Created/Modified

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Core/Interfaces/IPlugin.cs` | Créé | Interface base `string Name { get; }` |
| `src/SDK.Core/Interfaces/IBattlePlugin.cs` | Modifié | Ajout `: IPlugin`, retrait `Name` (hérité) |
| `src/SDK.Battle/Plugins/PluginRegistry.cs` | Modifié | `List<IPlugin>`, `Register(IPlugin)`, `OfType<IBattlePlugin>()` |
| `src/plugins/SDK.Plugins.Nuzlocke/SDK.Plugins.Nuzlocke.csproj` | Créé | Projet Nuzlocke |
| `src/plugins/SDK.Plugins.Nuzlocke/NuzlockePlugin.cs` | Créé | Callback `nuzlocke_dead_{SpeciesId}` sur OnPokemonFainted |
| `src/plugins/SDK.Plugins.Randomizer/SDK.Plugins.Randomizer.csproj` | Créé | Projet Randomizer |
| `src/plugins/SDK.Plugins.Randomizer/RandomizerPlugin.cs` | Créé | RandomizePokemon() — Type1Id/Type2Id, seed déterministe |
| `src/plugins/SDK.Plugins.Turbo/SDK.Plugins.Turbo.csproj` | Créé | Projet Turbo |
| `src/plugins/SDK.Plugins.Turbo/TurboPlugin.cs` | Créé | IsActive flag, tous hooks no-op |
| `tests/SDK.Plugins.Nuzlocke.Tests/SDK.Plugins.Nuzlocke.Tests.csproj` | Créé | Projet tests (refs 3 plugins + SDK.Battle) |
| `tests/SDK.Plugins.Nuzlocke.Tests/NuzlockePluginTests.cs` | Créé | 3 tests AC-1 + AC-2 |
| `tests/SDK.Plugins.Nuzlocke.Tests/AllPluginsSmokeTests.cs` | Créé | 2 tests smoke AC-4 |
| `tests/SDK.Plugins.Randomizer.Tests/SDK.Plugins.Randomizer.Tests.csproj` | Créé | Projet tests Randomizer |
| `tests/SDK.Plugins.Randomizer.Tests/RandomizerPluginTests.cs` | Créé | 3 tests AC-3 |
| `src/SDK.MonoGame/Program.cs` | Modifié | PluginRegistry DI singleton + commentaires activation |
| `PokemonSDK.slnx` | Modifié | 5 projets ajoutés (chemins src/plugins/) |

## Decisions Made

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `IPlugin.Name` seul membre | Base minimaliste — sous-interfaces ajoutent les hooks par domaine | Évite la god-interface, extensible sans breaking change |
| `AllPluginsSmokeTests` dans Nuzlocke.Tests | Nuzlocke.Tests réfère déjà les 3 plugins — pas de 4e projet test pour Turbo seul | Moins de projets, même couverture |
| `src/plugins/` sous-dossier physique | 3 plugins existants + N à venir — structure anticipée maintenant | Convention définitive, zéro migration future |
| `SDK.MonoGame.csproj` : zéro ref `SDK.Plugins.*` | PluginRegistry disponible via SDK.Battle transitif — boundary respectée | D-06 et boundary respectés |

## Deviations from Plan

### Résumé

| Type | Nombre | Impact |
|------|--------|--------|
| Scope addition post-APPLY | 1 | Positif — structure pérennisée |
| Auto-fixed | 0 | — |
| Différés | 0 | — |

**Impact global :** Zéro régression, amélioration structure.

### Scope Addition: Réorganisation `src/plugins/`

- **Découvert après :** APPLY complet, même session
- **Contexte :** 3 plugins créés dans `src/SDK.Plugins.*/` (conforme plan) — identifié que `src/` allait s'encombrer avec N plugins futurs
- **Action :** `mv src/SDK.Plugins.* src/plugins/` + patch chemins csproj + slnx + STATE.md
- **Vérification :** Build 0 erreurs, 116 tests verts après déplacement
- **Statut :** Convention documentée dans STATE.md "Décisions émergentes (Plan 05-02)"

## Vérification Finale

```
dotnet build PokemonSDK.slnx  → Build succeeded. 0 Error(s), 0 Warning(s)
dotnet test PokemonSDK.slnx   → 116 tests, 0 failed
  SDK.Core.Tests        : 28 passed
  SDK.Scripting.Tests   : 10 passed
  SDK.Battle.Tests      : 41 passed
  SDK.Plugins.Nuzlocke.Tests  :  5 passed
  SDK.Plugins.Randomizer.Tests:  3 passed
  SDK.MonoGame.Tests    :  5 passed
  SDK.Data.Tests        : 24 passed
dotnet list SDK.Core package  → (vide — D-01 intact)
grep PluginRegistry Program.cs → match présent
dotnet run -- --headless      → exit 0
```

## Next Phase Readiness

**Prêt :**
- Architecture plugin multi-surface opérationnelle — IEncounterPlugin (Phase 6) et IMapPlugin (Phase 7) peuvent s'ajouter sans modifier PluginRegistry
- Convention `src/plugins/` établie — prochain plugin : créer `src/plugins/SDK.Plugins.{Nom}/`, 2 refs csproj, ajouter au slnx
- D-13 satisfait — Nuzlocke/Randomizer/Turbo = plugins Register()-és, jamais modes hardcodés

**Points de vigilance pour 05-03 (Characters) :**
- D-22 : CharacterDataSeeder doit seed 6 locales (`CharacterTranslationsD22Tests` obligatoire)
- EntityType = "Character" / "VillainGroup" (PascalCase, cohérent avec "Badge", "Move")
- Characters en SDK.Core.Entities + EF migration dans SDK.Data (pas de plugin pour cette phase)

**Blockers :** Aucun.

---
*Phase: 05-plugins-characters, Plan: 02*
*Complété: 2026-06-06*
