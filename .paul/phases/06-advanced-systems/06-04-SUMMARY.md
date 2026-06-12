---
phase: 06-advanced-systems
plan: 04
subsystem: plugin
tags: [tts, narration, plugin, channel, cross-platform, process]

requires:
  - phase: 06-advanced-systems/06-01
    provides: FakemonSpecies (parallel, no dep)

provides:
  - INarrationPlugin interface (SDK.Core.Interfaces — zéro dep NuGet)
  - SDK.Plugins.TTS project complet (NullNarrationPlugin, NarrationQueue, WindowsSpeechPlugin, PiperTtsPlugin, NarrationPluginFactory)
  - SDK.Plugins.TTS.Tests — 20 tests cross-platform

affects: [06-05-tts-lua-binding]

tech-stack:
  added:
    - System.Threading.Channels (BCL .NET 10 — zéro NuGet)
    - System.Diagnostics.Process (BCL — PiperTtsPlugin + WindowsSpeechPlugin)
  patterns:
    - "NarrationQueue: Channel<string>.CreateUnbounded + Task.Run ProcessAsync — non-blocking"
    - "PiperTtsPlugin: launch piper process → aplay/paplay/mplayer — temp file pipeline"
    - "WindowsSpeechPlugin: PowerShell + System.Speech via process (cross-platform compile, Windows runtime)"
    - "NarrationPluginFactory: Windows → Piper → Null fallback chain"

key-files:
  created:
    - src/SDK.Core/Interfaces/INarrationPlugin.cs
    - src/plugins/SDK.Plugins.TTS/SDK.Plugins.TTS.csproj
    - src/plugins/SDK.Plugins.TTS/NullNarrationPlugin.cs
    - src/plugins/SDK.Plugins.TTS/NarrationQueue.cs
    - src/plugins/SDK.Plugins.TTS/NarrationPluginFactory.cs
    - src/plugins/SDK.Plugins.TTS/WindowsSpeechPlugin.cs
    - src/plugins/SDK.Plugins.TTS/PiperTtsPlugin.cs
    - tests/SDK.Plugins.TTS.Tests/SDK.Plugins.TTS.Tests.csproj
    - tests/SDK.Plugins.TTS.Tests/NullNarrationPluginTests.cs
    - tests/SDK.Plugins.TTS.Tests/NarrationQueueTests.cs
    - tests/SDK.Plugins.TTS.Tests/PiperTtsPluginTests.cs
    - tests/SDK.Plugins.TTS.Tests/NarrationPluginFactoryTests.cs
  modified:
    - PokemonSDK.slnx

key-decisions:
  - "WindowsSpeechPlugin via PowerShell process (pas System.Speech NuGet) — compile cross-platform, parle sur Windows"
  - "NarrationQueue: Channel<string> + Task.Run uniquement — pas IHostedService/BackgroundService (MonoGame context)"
  - "PiperTtsPlugin.IsSupported: File.Exists(path) || IsOnPath(path) — check PATH exact"
  - "DeterminePlayer: aplay → paplay → mplayer → null — ordre priorité audio Linux"

patterns-established:
  - "Plugin TTS suit même pattern que SDK.Plugins.Nuzlocke : src/plugins/ + tests/ flat"
  - "INarrationPlugin.Enqueue = fire-and-forget (no-op ou SpeakAsync fire) — NarrationQueue est le wrapper non-bloquant recommandé"

duration: ~35min
started: 2026-06-12T16:30:00Z
completed: 2026-06-12T17:05:00Z
---

# Phase 6 Plan 04: INarrationPlugin + SDK.Plugins.TTS — Summary

**INarrationPlugin interface + SDK.Plugins.TTS complet (Null/Queue/Windows/Piper/Factory) + 20 tests cross-platform, 0 régression.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~35 min |
| Démarré | 2026-06-12T16:30Z |
| Terminé | 2026-06-12T17:05Z |
| Tâches | 2/2 complètes |
| Fichiers créés | 13 (12 code + 1 slnx modifié) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: INarrationPlugin — SDK.Core 0 NuGet | ✅ PASS | `dotnet list SDK.Core.csproj package` → "No packages were found" |
| AC-2: NarrationQueue non-bloquant | ✅ PASS | Enqueue × 2 retourne en < 50ms avec mock 200ms |
| AC-3: Factory fallback NullNarrationPlugin si piper absent | ✅ PASS | Linux CI : piper absent → NullNarrationPlugin retourné |
| AC-4: PiperTtsPlugin.IsSupported=false si piper absent | ✅ PASS | `/nonexistent/piper` → IsSupported=false, SpeakAsync no-op |

## Accomplissements

- `INarrationPlugin` dans `SDK.Core.Interfaces` — contrat stable : EngineName, IsSupported, IsSpeaking, SpeakAsync, Stop, Enqueue
- `NarrationQueue` — `Channel<string>.CreateUnbounded` + `Task.Run ProcessAsync` avec catch `OperationCanceledException`
- `WindowsSpeechPlugin` — parle via PowerShell process (pas System.Speech NuGet) → compile sur Linux CI, parle sur Windows
- `PiperTtsPlugin` — pipeline temp-file : piper → wav → aplay/paplay/mplayer, cleanup finally
- `NarrationPluginFactory.CreateForCurrentPlatform` — fallback chain Windows → Piper → Null
- **20 tests** : NullNarrationPlugin ×6, NarrationQueue ×4, PiperTtsPlugin ×6, Factory ×4

## Fichiers Créés/Modifiés

| Fichier | Changement | But |
|---------|-----------|-----|
| `src/SDK.Core/Interfaces/INarrationPlugin.cs` | Créé | Contrat TTS cross-plugin |
| `src/plugins/SDK.Plugins.TTS/SDK.Plugins.TTS.csproj` | Créé | Projet NuGet packable v0.1.0 |
| `src/plugins/SDK.Plugins.TTS/NullNarrationPlugin.cs` | Créé | Fallback no-op, IsSupported=true |
| `src/plugins/SDK.Plugins.TTS/NarrationQueue.cs` | Créé | Wrapper non-bloquant Channel<string> |
| `src/plugins/SDK.Plugins.TTS/NarrationPluginFactory.cs` | Créé | Factory + NarrationOptions record |
| `src/plugins/SDK.Plugins.TTS/WindowsSpeechPlugin.cs` | Créé | SAPI via PowerShell, cross-compile |
| `src/plugins/SDK.Plugins.TTS/PiperTtsPlugin.cs` | Créé | Piper TTS + audio player |
| `tests/SDK.Plugins.TTS.Tests/*.cs` (×4) | Créé | 20 tests xUnit |
| `PokemonSDK.slnx` | Modifié | 2 projets ajoutés |

## Décisions

| Décision | Rationale | Impact |
|----------|-----------|--------|
| WindowsSpeechPlugin = PowerShell process | System.Speech NuGet Windows-only casse Linux CI | Compile partout, parle sur Windows |
| Channel<string> BCL uniquement | IHostedService/BackgroundService = ASP.NET context; MonoGame = standalone | Zéro dep ajoutée à SDK.Plugins.TTS |
| IsOnPath() via PATH split | File.Exists seul ne détecte pas les binaires dans $PATH | `piper` (sans path absolu) correctement détecté |

## Déviations

Aucune — plan exécuté tel que spécifié.

## Next Phase Readiness

**Prêt :**
- `INarrationPlugin` stable → 06-05 peut ajouter le binding Lua + DoctorCommand
- `NarrationQueue` testée → game dev peut l'utiliser immédiatement

**Concerns :** Aucun.

**Blockers :** Aucun.

---
*Phase: 06-advanced-systems, Plan: 04*
*Complété: 2026-06-12*
