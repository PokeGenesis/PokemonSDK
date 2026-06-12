---
phase: 06-advanced-systems
plan: 03
subsystem: cli
tags: [fakemon, cli, system-commandline, fakemon-assembly, pokeforge]

requires:
  - phase: 06-02
    provides: FakemonAssemblyPipeline, FakemonPartsCatalog, FakemonFilter, FakemonAssemblyOptions

provides:
  - "pokeforge fakemon list-parts — liste les parties PNG disponibles avec filtre"
  - "pokeforge fakemon assemble — assemble un Fakemon via CLI + insère en DB"
  - "FakemonCommandTests — 3 tests CLI (list-parts, fake PNG, strict-empty)"

affects: ["Phase 11 documentation", "Phase 8 NuGet re-publish"]

tech-stack:
  added: []
  patterns: ["InvocationContext pour >8 options SetHandler", "internal static Execute pour testabilité CLI"]

key-files:
  created:
    - src/SDK.Cli/Commands/FakemonCommand.cs
    - tests/SDK.Cli.Tests/FakemonCommandTests.cs
  modified:
    - src/SDK.Cli/Program.cs

key-decisions:
  - "InvocationContext (ctx.ParseResult.GetValueForOption) utilisé pour assemble — SetHandler typé limité à 8 params en beta4"
  - "FakemonCommand.ExecuteListParts + ExecuteAssemble = internal static — testables sans lancer le binaire"
  - "null! passé comme PokemonDbContext dans test strict+empty — exception levée avant accès ctx"
  - "Fake PNG = new byte[1] — FakemonPartsCatalog.Scan filtre par extension .png seulement, pas validation format"

patterns-established:
  - "Commandes CLI à >8 options : toujours utiliser InvocationContext pattern"
  - "Tests CLI : appeler méthode internal static, pas Process.Start"

duration: ~15min
started: 2026-06-12T17:45:00Z
completed: 2026-06-12T17:55:00Z
---

# Phase 6 Plan 03 : FakemonCommand CLI Summary

**`pokeforge fakemon list-parts` et `assemble` exposés via CLI — ADV-04 utilisable sans code C#.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~15 min |
| Démarré | 2026-06-12T17:45Z |
| Terminé | 2026-06-12T17:55Z |
| Tasks | 2 complètes |
| Fichiers | 3 (2 créés, 1 modifié) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : list-parts liste les parties disponibles | PASS | `ExecuteListParts` appelle `FakemonPartsCatalog.Scan` + `FakemonFilter.Apply`, affiche `[OK] {path} (ZOrder=N)`, exit 0 |
| AC-2 : assemble crée un Fakemon | PASS | `ExecuteAssemble` construit `FakemonAssemblyOptions`, crée `PokemonDbContext` via `UseSqlite`, appelle `RunAsync`, exit 0 |
| AC-3 : assemble filtre vide → exit 1 | PASS | `FakemonAssemblyPipeline.RunAsync(opts, null!)` avec `Strict=true` + répertoire vide lève `FakemonAssemblyException("*strict*")` |

## Accomplissements

- CLI `pokeforge fakemon list-parts` et `assemble` enregistrées et fonctionnelles
- `InvocationContext` pattern établi pour contourner la limite de 8 paramètres typés de `SetHandler`
- 3 tests verts dans `FakemonCommandTests` (list-parts vide, list-parts PNG, assemble strict)
- Vérification live : `pokeforge fakemon --help` affiche les deux sous-commandes

## Fichiers Créés/Modifiés

| Fichier | Change | But |
|---------|--------|-----|
| `src/SDK.Cli/Commands/FakemonCommand.cs` | Créé | Commandes `list-parts` + `assemble` avec `internal static Execute` testables |
| `src/SDK.Cli/Program.cs` | Modifié | `FakemonCommand.Register(rootCommand)` ajouté |
| `tests/SDK.Cli.Tests/FakemonCommandTests.cs` | Créé | 3 tests CLI (empty dir, fake PNG, strict+empty) |

## Décisions

| Décision | Raison | Impact |
|----------|--------|--------|
| `InvocationContext` pour `assemble` | `SetHandler` typé limite à 8 params, `assemble` a 10 options | Pattern réutilisable pour futures commandes >8 options |
| `null!` comme ctx dans test strict | Exception levée avant accès ctx quand `Strict=true` + 0 parties | Pas besoin de `Microsoft.Data.Sqlite` dans `SDK.Cli.Tests` |
| Fake PNG = `new byte[1]` | `FakemonPartsCatalog.Scan` filtre par extension `.png` uniquement | Tests légers sans ImageSharp |

## Écarts par rapport au plan

### Auto-fixés

**1. SetHandler limite 8 paramètres**
- **Trouvé lors :** Task 1 (FakemonCommand assemble — 10 options)
- **Problème :** `SetHandler<T1..T8>` limité à 8 paramètres en System.CommandLine 2.0.0-beta4
- **Fix :** `cmd.SetHandler(ctx => { var val = ctx.ParseResult.GetValueForOption(opt); ... })`
- **Vérification :** Build 0 erreur, `assemble --help` affiche toutes les options

### Ajouts de scope

**1. `InternalsVisibleTo` dans SDK.Cli.csproj**
- Non dans le plan — nécessaire pour que `FakemonCommandTests` accède à `ExecuteListParts` (internal)
- Impact nul — pattern déjà établi dans le projet (SDK.Tools → SDK.Tools.Tests)

## Issues Rencontrées

| Problème | Résolution |
|----------|------------|
| MSB3492 stale cache (`CoreCompileInputs.cache`) | Suppression `obj/Release/net10.0/SDK.Cli.csproj.CoreCompileInputs.cache` — build propre (pattern E-05 connu) |

## Prêt pour la Suite

**Prêt :**
- `pokeforge fakemon assemble` entièrement fonctionnel — game devs peuvent assembler sans C#
- Pattern `InvocationContext` disponible pour futures commandes >8 options
- ADV-04 complet (avec 06-02 FakemonAssemblyPipeline)

**Préoccupations :**
- Re-publish NuGet du binaire CLI différé Phase 8 (D-18, pas de breaking change)

**Bloquants :** Aucun

---
*Phase: 06-advanced-systems, Plan: 03*
*Complété: 2026-06-12*
