---
phase: 10-cli-pokeforge
plan: 01
subsystem: cli
tags: [dotnet-tool, system-commandline, zip-extract, scaffolding, nuget]

requires:
  - phase: 09-sample-project
    provides: StarterGame complet (13 fichiers source) utilisé comme template embarqué

provides:
  - SDK.Cli dotnet global tool `pokeforge` (PackAsTool=true, ToolCommandName=pokeforge)
  - Commande `pokeforge new <name>` extrait + renomme un projet MonoGame depuis un zip embarqué
  - starter-template.zip (13 fichiers, 0 bin/obj) embarqué en EmbeddedResource
  - Suite de tests NewCommandTests (7 cas)

affects: [10-02-asset-sync-seed, 10-03-doctor-publish]

tech-stack:
  added:
    - System.CommandLine 2.0.0-beta4.22272.1
    - xunit 2.9.2 (tests)
    - FluentAssertions 8.10.0 (tests)
  patterns:
    - PackAsTool + EmbeddedResource pour template CLI
    - InternalsVisibleTo via AssemblyAttribute dans .csproj (sans AssemblyInfo.cs)
    - IDisposable + Directory.SetCurrentDirectory pour isolation des tests CWD-sensibles
    - Environment.Exit(Execute(name)) — contournement beta4 SetHandler sans retour int

key-files:
  created:
    - src/SDK.Cli/SDK.Cli.csproj
    - src/SDK.Cli/Program.cs
    - src/SDK.Cli/Commands/NewCommand.cs
    - src/SDK.Cli/Templates/starter-template.zip
    - tests/SDK.Cli.Tests/SDK.Cli.Tests.csproj
    - tests/SDK.Cli.Tests/NewCommandTests.cs
  modified:
    - PokemonSDK.slnx

key-decisions:
  - "zip exclusion via find+prune (pas zip --exclude) — seule approche excluant bin/obj à tous les niveaux"
  - "nuget.config écrit en dur dans Execute() pour éliminer la source local-pokeforge de dev"
  - "Environment.Exit(Execute(name)) — System.CommandLine beta4 SetHandler ne supporte pas Func<int>"

patterns-established:
  - "Test isolation CWD : IDisposable avec tempDir GUID + SetCurrentDirectory"
  - "InternalsVisibleTo dans .csproj — pattern réutilisable pour SDK.Cli.Tests en Phase 10-02/03"

duration: ~60min
started: 2026-06-08T19:50:00Z
completed: 2026-06-08T21:00:00Z
---

# Phase 10 Plan 01: pokeforge CLI — `new` Command Summary

**`pokeforge new <name>` : dotnet global tool extrayant StarterGame depuis un zip embarqué, renommant StarterGame → PascalCase(name) dans tous les fichiers texte, copiant les binaires tels quels.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~60 min |
| Démarré | 2026-06-08T19:50Z |
| Complété | 2026-06-08T21:00Z |
| Tâches | 3/3 complètes |
| Fichiers créés/modifiés | 7 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: `new` crée le projet | **Pass** | `test-game/TestGame.csproj` créé, 0 `StarterGame` dans .cs/.csproj |
| AC-2: Conversion PascalCase | **Pass** | `mon-jeu`→`MonJeu`, `single`→`Single`, `my-pokemon-game`→`MyPokemonGame` |
| AC-3: Erreur dossier existant | **Pass** | exit code 1, message stderr, aucun fichier créé |
| AC-4: `--help` fonctionnel | **Pass** | System.CommandLine gère automatiquement |
| AC-5: Binaires copiés sans modification | **Pass** | `bgm.ogg` byte-identique, vérifié par test |

## Accomplissements

- `src/SDK.Cli/` créé — dotnet global tool `pokeforge` prêt pour `dotnet tool pack`
- `starter-template.zip` (13 fichiers, 0 bin/obj) embarqué via EmbeddedResource — résolu le problème d'exclusion nested `Content/bin/` et `Content/obj/` avec `find+prune`
- `NewCommand.Execute()` : extraction ZipArchive, ToPascalCase, rename `.csproj`, nuget.config propre, passthrough binaire
- 7/7 tests unitaires verts — isolation CWD via IDisposable + tempDir GUID

## Fichiers Créés/Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Cli/SDK.Cli.csproj` | Créé | PackAsTool, ToolCommandName=pokeforge, EmbeddedResource zip |
| `src/SDK.Cli/Program.cs` | Créé | Entry point top-level System.CommandLine |
| `src/SDK.Cli/Commands/NewCommand.cs` | Créé | ToPascalCase + Execute + Register |
| `src/SDK.Cli/Templates/starter-template.zip` | Créé | 13 fichiers source StarterGame sans bin/obj |
| `tests/SDK.Cli.Tests/SDK.Cli.Tests.csproj` | Créé | xUnit + FluentAssertions + ProjectRef SDK.Cli |
| `tests/SDK.Cli.Tests/NewCommandTests.cs` | Créé | 7 tests : PascalCase (3) + Execute (4) |
| `PokemonSDK.slnx` | Modifié | SDK.Cli + SDK.Cli.Tests ajoutés |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `find+prune` pour le zip | `zip --exclude "*/bin/*"` ne couvre pas les sous-dossiers comme `Content/bin/` | Template propre 13 fichiers |
| `nuget.config` écrit en dur | Source `local-pokeforge` du dev feed inutilisable chez les makers | Generated project prêt OOB |
| `Environment.Exit(Execute(name))` | System.CommandLine beta4 SetHandler ne supporte pas `Func<int>` | Workaround documenté, plan 10-02/03 doit suivre ce pattern |
| InternalsVisibleTo via `AssemblyAttribute` dans .csproj | Évite AssemblyInfo.cs redondant | Pattern réutilisable |

## Déviations du Plan

### Résumé

| Type | Nombre | Impact |
|------|--------|--------|
| Auto-fixés | 2 | Correctifs essentiels |
| Ajouts de scope | 0 | Aucun |
| Différés | 0 | Aucun |

**Impact total :** Correctifs techniques essentiels, aucun scope creep.

### Auto-fixés

**1. Exclusion zip nested bin/obj**
- **Détecté pendant :** Task 1 (création du zip)
- **Problème :** `zip --exclude "*/bin/*"` n'exclut pas `Content/bin/` et `Content/obj/` (build artifacts MGCB) — le zip contenait 200+ fichiers au lieu de 13
- **Fix :** `find . \( -path "*/bin" -o -path "*/obj" \) -prune -o -type f -print | zip ... -@`
- **Vérification :** `unzip -l starter-template.zip` → 13 fichiers uniquement

**2. Ajout `InternalsVisibleTo` dans SDK.Cli.csproj**
- **Détecté pendant :** Task 3 (tests)
- **Problème :** `NewCommand.Execute()` et `ToPascalCase()` sont `internal static` — tests dans assembly séparée ne peuvent y accéder sans `InternalsVisibleTo`
- **Fix :** `<AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">` dans SDK.Cli.csproj
- **Vérification :** Tests compilent et accèdent directement aux méthodes internes

## Résultats de Vérification

```
dotnet build PokemonSDK.slnx
→ Build succeeded. 0 Warning(s). 0 Error(s).

dotnet test tests/SDK.Cli.Tests/
→ Passed! Failed: 0, Passed: 7, Skipped: 0, Total: 7, Duration: 40ms
```

## Préparation Phase Suivante

**Prêt :**
- `PokeForge.Cli` namespace établi — plans 10-02/03 ajoutent `AssetSyncCommand`, `SeedCommand`, `DoctorCommand` dans `src/SDK.Cli/Commands/`
- Pattern `Register(RootCommand root)` + `Environment.Exit(Execute(...))` documenté — à réutiliser pour toutes les sous-commandes
- `InternalsVisibleTo` déjà configuré — tests Plan 10-02/03 accèdent aux internals sans configuration supplémentaire
- `IDisposable` + tempDir GUID — pattern de test réutilisable

**Concerns :**
- System.CommandLine beta4 — utiliser `Environment.Exit` partout (pas de retour int dans SetHandler). À reconsidérer si version stable disponible lors du Plan 10-03.
- `starter-template.zip` snapshot de StarterGame au 2026-06-08 — si StarterGame évolue dans Phase 11+, il faudra mettre à jour le zip manuellement.

**Blockers :**
- Aucun.

---
*Phase: 10-cli-pokeforge, Plan: 01*
*Complété: 2026-06-08*
