---
phase: 06-advanced-systems
plan: 05
subsystem: scripting
tags: [tts, lua, moonsharp, narration, doctor, piper, aplay]

requires:
  - phase: 06-04
    provides: INarrationPlugin interface, SDK.Plugins.TTS (PiperNarrationPlugin)

provides:
  - "TtsApi — binding MoonSharp sdk.tts.speak/stop/is_speaking → INarrationPlugin"
  - "NpcInteractionRunner — surcharge INarrationPlugin? optionnelle + SdkGlobals wrapper"
  - "DoctorCommand — vérification piper + aplay (WARN non-bloquant)"
  - "TtsApiTests — 3 tests (speak, stop, Lua end-to-end)"

affects: ["Phase 11 documentation", "Phase 12+ BattleScene (NPC dialogue avec TTS)"]

tech-stack:
  added: []
  patterns:
    - "SdkGlobals sealed class pour namespace Lua imbriqué (sdk.tts.*)"
    - "Méthodes C# en minuscules pour mappage direct des identifiants Lua"
    - "UserData.RegisterType<T>() explicite avant RegisterApi pour types imbriqués"

key-files:
  created:
    - src/SDK.Scripting/Bindings/TtsApi.cs
    - tests/SDK.Scripting.Tests/Bindings/TtsApiTests.cs
  modified:
    - src/SDK.Scripting/Bindings/NpcInteractionRunner.cs
    - src/SDK.Cli/Commands/DoctorCommand.cs

key-decisions:
  - "Méthodes TtsApi en minuscules (speak/stop/is_speaking) — MoonSharp mappe les noms C# sensible à la casse vers Lua"
  - "SdkGlobals public sealed class avec property 'tts' minuscule — seul moyen d'obtenir sdk.tts.speak() imbriqué"
  - "UserData.RegisterType<TtsApi>() obligatoire avant RegisterApi(sdk) — MoonSharp ne réfléchit pas les types imbriqués automatiquement"
  - "IsCommandAvailable utilise where/which selon OS — cross-platform sans dépendance externe"

patterns-established:
  - "Namespace Lua imbriqué (sdk.X.method) via SdkGlobals + property minuscule"
  - "Bindings Lua : méthodes en minuscules pour mappage direct (pas de MoonSharp rename attribute nécessaire)"

duration: ~15min
started: 2026-06-12T17:45:00Z
completed: 2026-06-12T17:55:00Z
---

# Phase 6 Plan 05 : TTS Lua Binding + DoctorCommand Check Summary

**TtsApi MoonSharp expose `sdk.tts.speak/stop/is_speaking` en Lua — ADV-03 complet. `pokeforge doctor` vérifie piper+aplay.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~15 min |
| Démarré | 2026-06-12T17:45Z |
| Terminé | 2026-06-12T17:55Z |
| Tasks | 2 complètes |
| Fichiers | 4 (2 créés, 2 modifiés) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : Lua sdk.tts.* délègue à INarrationPlugin | PASS | `sdk.tts.speak("bonjour")` → `INarrationPlugin.Enqueue("bonjour")` — vérifié par TtsApiTests.NpcInteractionRunner_LuaTtsSpeak_DelegatesEnqueue |
| AC-2 : Lua ne peut pas lancer de process | PASS | SoftSandbox déjà validé en Plan 04-01 (os.exit lève ScriptRuntimeException). Non dupliqué — référencé dans plan. |
| AC-3 : pokeforge doctor vérifie piper + aplay | PASS | `[WARN] piper TTS absent` si absent, `[OK] piper TTS disponible` si présent. Exit code 0 (WARN non-bloquant). aplay vérifié sur non-Windows uniquement. |

## Accomplissements

- `sdk.tts.speak("text")` / `sdk.tts.stop()` / `sdk.tts.is_speaking()` utilisables depuis tout script Lua via NpcInteractionRunner
- `SdkGlobals` pattern établi pour namespaces Lua imbriqués (`sdk.X.method`)
- `DoctorCommand` vérifie désormais la chaîne complète TTS (piper + aplay) avec WARN cross-platform
- 3 tests verts dont un test Lua end-to-end complet

## Fichiers Créés/Modifiés

| Fichier | Change | But |
|---------|--------|-----|
| `src/SDK.Scripting/Bindings/TtsApi.cs` | Créé | Wrapper INarrationPlugin exposant `speak`/`stop`/`is_speaking` (minuscules) pour Lua |
| `src/SDK.Scripting/Bindings/NpcInteractionRunner.cs` | Réécriture | Surcharge `INarrationPlugin? tts`, `SdkGlobals` class, `UserData.RegisterType<TtsApi>()` |
| `src/SDK.Cli/Commands/DoctorCommand.cs` | Modifié | `IsCommandAvailable` helper + checks piper/aplay WARN en fin de `Execute()` |
| `tests/SDK.Scripting.Tests/Bindings/TtsApiTests.cs` | Créé | 3 tests (speak→Enqueue, stop→Stop, Lua→Enqueue end-to-end) |

## Décisions

| Décision | Raison | Impact |
|----------|--------|--------|
| Méthodes TtsApi minuscules (`speak`, `stop`, `is_speaking`) | MoonSharp mappe noms C# sensible à la casse vers identifiants Lua | Pattern : toute future `*Api` doit avoir méthodes en minuscules pour mapping direct |
| `SdkGlobals sealed class` (property `tts` minuscule) | `engine.RegisterApi("sdk", obj)` expose seulement le type top-level ; propriété `tts` minuscule donne `sdk.tts` | Pattern réutilisable : `sdk.items`, `sdk.party`, etc. |
| `UserData.RegisterType<TtsApi>()` explicite | MoonSharp ne réfléchit pas les types CLR imbriqués automatiquement en SoftSandbox | Obligatoire pour tout type CLR dans namespace imbriqué |
| `NpcInteractionRunner.cs` réécriture complète | Ajout surcharge + SdkGlobals class dans même fichier — plus cohérent que fichier séparé | Aucun impact sur l'API existante (`Run(engine, state, script)` conservé) |

## Écarts par rapport au plan

### Auto-fixés

**1. Nommage TtsApi : PascalCase → minuscules**
- **Trouvé lors :** Task 1 (TtsApi.cs)
- **Problème :** Plan spécifiait `Speak()`, `Stop()`, `IsSpeaking` (PascalCase) — MoonSharp mappe les noms exact, donc `sdk.tts.Speak()` serait requis en Lua (majuscule)
- **Fix :** Méthodes renommées `speak`, `stop`, `is_speaking` — identifiants Lua standard (snake_case minuscule)
- **Vérification :** `TtsApiTests.NpcInteractionRunner_LuaTtsSpeak_DelegatesEnqueue` appelle `sdk.tts.speak('bonjour')` → passe

**2. SdkGlobals class (scope addition)**
- **Trouvé lors :** Task 1 (NpcInteractionRunner + RegisterApi)
- **Problème :** `engine.RegisterApi("sdk", new TtsApi(tts))` donnerait `sdk.speak()` pas `sdk.tts.speak()`
- **Fix :** `SdkGlobals { public TtsApi tts }` class ajoutée — pas prévue dans le plan
- **Vérification :** Test Lua `sdk.tts.speak('bonjour')` passe

## Issues Rencontrées

| Problème | Résolution |
|----------|------------|
| MSB3492 stale cache (`CoreCompileInputs.cache`) | Suppression `obj/Release/net10.0/SDK.Scripting.csproj.CoreCompileInputs.cache` — build propre |

## Prêt pour la Suite

**Prêt :**
- ADV-03 TTS narration 100% complet : INarrationPlugin (06-04) + TtsApi Lua binding (06-05) + DoctorCommand check
- Pattern `SdkGlobals` réutilisable pour `sdk.items`, `sdk.party` (Phases 14-15)
- Phase 6 entièrement complète (5/5 plans APPLY+UNIFY)

**Préoccupations :**
- `aplay` requis sur Linux pour audio TTS — `pokeforge doctor` avertit l'utilisateur

**Bloquants :** Aucun

---
*Phase: 06-advanced-systems, Plan: 05*
*Complété: 2026-06-12*
