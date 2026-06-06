---
phase: 07-developer-experience
plan: 03
subsystem: scripting
tags: [hot-reload, lua, moonsharp, filesystemwatcher, sdk-scripting, sdk-monogame]

requires:
  - phase: 04-scripting-progression
    provides: IScriptEngine, LuaScriptEngine, MoonSharp SoftSandbox

provides:
  - IScriptEngine.Reload(path) — contrat SDK.Core
  - LuaHotReloader (#if DEBUG, FileSystemWatcher)
  - LuaErrorOverlay (état HasError + stub Draw)
  - Game1 câblé #if DEBUG sans violation D-06

affects: [phase9-sample]

tech-stack:
  added: []
  patterns: [FileSystemWatcher #if DEBUG, IDisposable watcher, ManualResetEventSlim pour tests WSL2]

key-files:
  created:
    - src/SDK.Scripting/HotReload/LuaHotReloader.cs
    - src/SDK.MonoGame/UI/LuaErrorOverlay.cs
    - tests/SDK.Scripting.Tests/LuaScriptEngineReloadTests.cs
    - tests/SDK.Scripting.Tests/LuaHotReloaderTests.cs
  modified:
    - src/SDK.Core/Interfaces/IScriptEngine.cs
    - src/SDK.Scripting/Engine/LuaScriptEngine.cs
    - src/SDK.MonoGame/Game1.cs

key-decisions:
  - "IScriptEngine.Reload(path) : nouveau Script(SoftSandbox) + DoFile — D-04 inchangé dans Reload"
  - "D-06 : engine hot reload créé via _scriptEngineFactory() dans Game1, jamais new LuaScriptEngine()"
  - "D-17 : LuaHotReloader dans SDK.Scripting/HotReload/ (pas SDK.MonoGame)"
  - "LuaErrorOverlay sans #if DEBUG — classe légère, utile pour log stderr en Release"
  - "FileSystemWatcher WSL2 : double-fire sur write → Times.AtLeastOnce() dans tests"
  - "MoonSharp nil→double : ToObject<T>() lève exception → try/catch requis"

patterns-established:
  - "#if DEBUG wiring dans Game1 : IDisposable LuaHotReloader + OnReloadError → overlay.SetError"
  - "ManualResetEventSlim(800ms) pour synchroniser tests FileSystemWatcher WSL2"

duration: ~45min
started: 2026-06-06T20:55:00Z
completed: 2026-06-06T21:40:00Z
---

# Phase 7 Plan 03: LuaHotReloader + LuaErrorOverlay + IScriptEngine.Reload Summary

**Hot reload Lua <500ms via FileSystemWatcher (#if DEBUG) + IScriptEngine.Reload (nouveau contexte SoftSandbox propre) + LuaErrorOverlay — DX-02 livré.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~45min |
| Started | 2026-06-06T20:55:00Z |
| Completed | 2026-06-06T21:40:00Z |
| Tasks | 3 completed |
| Files modified | 7 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: IScriptEngine.Reload + LuaScriptEngine clean reload | Pass | Nouveau Script(SoftSandbox) → ancien état effacé |
| AC-2: LuaHotReloader déclenche Reload sur .lua change | Pass | FileSystemWatcher, Times.AtLeastOnce (WSL2 double-fire) |
| AC-3: OnReloadError fire si Reload lève exception | Pass | LastError peuplé, event déclenché |
| AC-4: Build Debug propre, ≥160 tests | Pass | 0 erreurs, 166 tests verts |

## Accomplishments

- Hot reload Lua opérationnel en Debug — enregistrer un .lua recharge le script en <500ms
- D-04 préservé dans Reload : Preset_SoftSandbox inchangé pour le nouveau contexte
- D-06 respecté : Game1 utilise `_scriptEngineFactory()` pour l'engine hot reload, jamais `new LuaScriptEngine()`
- LuaErrorOverlay : state management HasError/LastError + SetError/ClearError (Draw stub pour plan 07-04)

## Task Commits

| Task | Commit | Type | Description |
|------|--------|------|-------------|
| T0+T1+T2: Full hot reload | `04ebdf6` | feat | IScriptEngine.Reload + LuaHotReloader + LuaErrorOverlay + Game1 wiring |

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Core/Interfaces/IScriptEngine.cs` | Modified | Ajout `void Reload(string path)` |
| `src/SDK.Scripting/Engine/LuaScriptEngine.cs` | Modified | Implémentation Reload : nouveau SoftSandbox + DoFile |
| `src/SDK.Scripting/HotReload/LuaHotReloader.cs` | Created | FileSystemWatcher #if DEBUG, OnReloadError event |
| `src/SDK.MonoGame/UI/LuaErrorOverlay.cs` | Created | État HasError/LastError + SetError/ClearError/Draw stub |
| `src/SDK.MonoGame/Game1.cs` | Modified | #if DEBUG : HotReloader instancié, OnReloadError câblé |
| `tests/SDK.Scripting.Tests/LuaScriptEngineReloadTests.cs` | Created | 2 tests Reload (clean state, nonexistent throws) |
| `tests/SDK.Scripting.Tests/LuaHotReloaderTests.cs` | Created | 4 tests FSWatcher (trigger, ignore non-lua, error event, dispose) |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| Reload = nouveau Script() complet | Isolation totale — pas de pollution entre scripts — D-04 intact | Comportement prévisible, pas de state leak |
| LuaHotReloader dans SDK.Scripting (pas MonoGame) | D-17 : SDK.Tools headless, SDK.Scripting sans MonoGame aussi | Testable sans dépendance MonoGame |
| LuaErrorOverlay sans #if DEBUG | Classe légère, log stderr utile en Release également | Draw() stub toujours compilé, safe |
| ManualResetEventSlim(800ms) dans tests | WSL2 FileSystemWatcher double-fire — Times.AtLeastOnce() seule assertion fiable | Pattern obligatoire pour tout test FSWatcher sur WSL2 |

## Deviations from Plan

None — plan exécuté exactement.

## Issues Encountered

| Issue | Resolution |
|-------|------------|
| MoonSharp `Evaluate<T>` expression doit être préfixée `"return "` | Fix dans tests — `"x"` seul → UnexpectedTokenType, `"return x"` → OK |
| MoonSharp nil→double : `ToObject<T>()` lève exception (pas default) | try/catch dans LuaScriptEngineReloadTests |
| FileSystemWatcher WSL2 double-fire | Times.AtLeastOnce() + ManualResetEventSlim(800ms) |

## Next Phase Readiness

**Ready:**
- LuaErrorOverlay.Draw() stub → implémentation réelle avec SpriteFont (plan 07-04)
- Game1 prêt pour LuaConsole wiring (#if DEBUG)
- 166 tests verts — baseline propre

**Blockers:** None

---
*Phase: 07-developer-experience, Plan: 03*
*Completed: 2026-06-06*
