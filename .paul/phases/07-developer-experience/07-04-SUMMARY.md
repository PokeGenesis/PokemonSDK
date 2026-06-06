---
phase: 07-developer-experience
plan: 04
subsystem: monogame
tags: [lua-console, repl, mgcb, spritefont, draw, monogame, sdk-monogame]

requires:
  - phase: 07-03
    provides: LuaErrorOverlay stub Draw, Game1 #if DEBUG wiring
  - phase: 07-02
    provides: MGCB pipeline SDK.Tools

provides:
  - LuaConsole REPL ingame (toggle ~, eval, history, SpriteBatch)
  - MGCB pipeline opérationnel (DefaultFont.xnb compilé)
  - Draw() réels : LuaErrorOverlay + DialogueBox + LuaConsole
  - Game1 fully wired (SpriteFont chargée, keyboard handling, LuaConsole DEBUG)

affects: [phase8-nuget, phase9-sample]

tech-stack:
  added: [dotnet-mgcb 3.8.4.1 (local tool), DejaVu Sans Mono (font Linux CI)]
  patterns: [MGCB Content pipeline DesktopGL, SpriteBatch overlay UI, #if DEBUG LuaConsole]

key-files:
  created:
    - src/SDK.MonoGame/UI/LuaConsole.cs
    - src/SDK.MonoGame/Content/Fonts/DefaultFont.spritefont
    - .config/dotnet-tools.json
    - tests/SDK.MonoGame.Tests/LuaConsoleTests.cs
  modified:
    - src/SDK.MonoGame/Content/Content.mgcb
    - src/SDK.MonoGame/UI/LuaErrorOverlay.cs
    - src/SDK.MonoGame/UI/DialogueBox.cs
    - src/SDK.MonoGame/Game1.cs

key-decisions:
  - "DejaVu Sans Mono pour DefaultFont.spritefont — installée nativement sur Ubuntu/Debian CI headless"
  - "Shaders xBR.fx + DayNight.fx commentés dans Content.mgcb — MGFXC requiert Wine 64-bit (manquant Ubuntu 24.04 t64)"
  - "RenderPipeline PointClamp fallback si .xnb shaders absents — null-safe, aucune régression"
  - "LuaConsole uniquement #if DEBUG — toggle ~, input TextInput, history circulaire"
  - "Game1 fields : SpriteFont _defaultFont, KeyboardState _prevKeyState, LuaConsole _luaConsole"

patterns-established:
  - "dotnet-mgcb tool local (.config/dotnet-tools.json) — restore via dotnet tool restore avant build CI"
  - "SpriteBatch overlay : Begin()/End() wrapping + DrawString pour UI ingame"
  - "LuaConsole pattern : accumulateur input + history List<string> + DrawString overlay"

duration: ~60min
started: 2026-06-06T21:40:00Z
completed: 2026-06-06T22:45:00Z
---

# Phase 7 Plan 04: LuaConsole REPL + MGCB Font + Draw() réels Summary

**LuaConsole REPL ingame (#if DEBUG, toggle ~) + MGCB pipeline opérationnel (DefaultFont.xnb) + Draw() réels SpriteBatch pour LuaErrorOverlay + DialogueBox — Phase 7 Developer Experience complète.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~60min |
| Started | 2026-06-06T21:40:00Z |
| Completed | 2026-06-06T22:45:00Z |
| Tasks | 4 completed |
| Files modified | 9 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: MGCB pipeline opérationnel | Pass | DefaultFont.xnb compilé, dotnet-mgcb tool local |
| AC-2: DefaultFont chargée dans Game1 | Pass | LoadContent + _defaultFont, 166 tests verts |
| AC-3: LuaConsole REPL fonctionnel | Pass | Toggle ~, input, submit, history, 6 tests verts |
| AC-4: Draw() réels sur tous les UI | Pass | LuaErrorOverlay + DialogueBox + LuaConsole SpriteBatch |

## Accomplishments

- MGCB pipeline opérationnel sur Linux/WSL2 — DefaultFont.xnb compilé et déployé dans output
- LuaConsole REPL ingame : toggle tilde, buffer TextInput, submit Enter, history Up/Down, error display
- Draw() réels remplacent tous les stubs no-op : LuaErrorOverlay + DialogueBox + LuaConsole
- 6 tests LuaConsoleTests verts (toggle, input, submit, history, error display, clear)

## Task Commits

| Task | Commit | Type | Description |
|------|--------|------|-------------|
| T0+T1+T2+T3: Full plan | `5e1e7df` | feat | LuaConsole + MGCB font + Draw() réels + Game1 wiring |

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.MonoGame/Content/Fonts/DefaultFont.spritefont` | Created | DejaVu Sans Mono 16pt — CI headless compatible |
| `.config/dotnet-tools.json` | Created | dotnet-mgcb 3.8.4.1 pinned comme local tool |
| `src/SDK.MonoGame/Content/Content.mgcb` | Modified | Font enregistrée, shaders commentés (Wine absent) |
| `src/SDK.MonoGame/UI/LuaConsole.cs` | Created | REPL ingame #if DEBUG, SpriteBatch overlay |
| `src/SDK.MonoGame/UI/LuaErrorOverlay.cs` | Modified | Draw() réel remplace stub |
| `src/SDK.MonoGame/UI/DialogueBox.cs` | Modified | Draw() réel remplace stub no-op |
| `src/SDK.MonoGame/Game1.cs` | Modified | SpriteFont chargée, _prevKeyState, _luaConsole #if DEBUG |
| `tests/SDK.MonoGame.Tests/LuaConsoleTests.cs` | Created | 6 tests (toggle, input, submit, history, error, clear) |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| DejaVu Sans Mono pour DefaultFont | Installée nativement sur Ubuntu/Debian — CI headless sans setup TTF supplémentaire | Font reproductible en CI |
| Shaders commentés dans Content.mgcb | MGFXC requiert wine64 — Ubuntu 24.04 t64 transition casse les dépendances apt wine64 | Shaders compilables en CI Windows (Phase CICD) |
| RenderPipeline PointClamp fallback | null-safe si .xnb shaders absents — aucune régression fonctionnelle | Game jouable sans shaders |
| LuaConsole uniquement #if DEBUG | REPL dev-only — pas de surface d'attaque en Release | #if DEBUG guards vérifiés Release build |

## Deviations from Plan

| Déviation | Impact |
|-----------|--------|
| Shaders xBR.fx + DayNight.fx NON compilés (Wine absent) | RenderPipeline PointClamp fallback actif — fonctionnel, visuellement dégradé. Compilable en CI Windows Phase CICD. |

## Issues Encountered

| Issue | Resolution |
|-------|------------|
| Wine64 absent Ubuntu 24.04 WSL2 (t64 transition casse libgd3:amd64) | Shaders commentés dans Content.mgcb, PointClamp fallback documenté |
| MGCB `/verbose` : chemin relatif ne fonctionne pas | Chemins absolus + `/@:` prefix pour response file |

## Next Phase Readiness

**Ready:**
- Phase 7 complète — DX-01 (asset pipeline) + DX-02 (hot reload + REPL) livrés
- 166 tests verts, build Debug + Release propres
- Phase 8 NuGet Distribution peut démarrer

**Concerns:**
- Shaders xBR + DayNight à compiler en CI Windows (Phase CICD)
- ImageSharp CVE à évaluer avant publication NuGet (Phase 8)
- FluentAssertions v8 licence Xceed à évaluer avant distribution commerciale

**Blockers:** None

---
*Phase: 07-developer-experience, Plan: 04*
*Completed: 2026-06-06*
