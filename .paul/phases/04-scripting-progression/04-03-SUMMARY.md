---
phase: 04-scripting-progression
plan: 03
subsystem: save-scripting-wiring
tags: [save-system, dialogue-box, game1, di-wiring, d-06, lua, codeql, cherry-pick]

requires:
  - phase: 04-scripting-progression/04-01
    provides: IScriptEngine, LuaScriptEngine, GameState
  - phase: 04-scripting-progression/04-02
    provides: BadgeApi, NpcInteractionRunner, badge_boulder flag
  - commit: 98c3299
    provides: CodeQL fixes (Game1 readonly, HeadlessRunner, RenderPipeline, WorldSystem)

provides:
  - SaveSystem (SDK.Core.Services) — ISaveSystem via System.Text.Json BCL, zéro NuGet
  - DialogueBox (SDK.MonoGame.UI) — état machine Open/Close, Draw() stub
  - gym_brock.lua — script prod Gen1 boulder badge
  - Game1.cs — _saveSystem + _scriptEngineFactory injectés via DI (D-06 compliant)
  - Program.cs — Func<IScriptEngine> factory + ISaveSystem enregistrés
  - 8 tests nouveaux (4 Core + 2 MonoGame + 2 Scripting)
affects: [phase5-plugins]

tech-stack:
  added: []
  patterns:
    - "SaveSystem : System.Text.Json BCL only — File.WriteAllText/ReadAllText, zéro NuGet ajouté à SDK.Core.csproj"
    - "Func<IScriptEngine> factory : AddSingleton<Func<IScriptEngine>>(_ => () => new LuaScriptEngine()) — engine frais par interaction"
    - "D-06 compliant : LuaScriptEngine visible UNIQUEMENT dans Program.cs (composition root) — jamais dans Game1.cs"
    - "Cherry-pick pattern : 98c3299 appliqué sur feature branch sans merge conflict"

key-files:
  created:
    - src/SDK.Core/Services/SaveSystem.cs
    - src/SDK.MonoGame/UI/DialogueBox.cs
    - src/SDK.MonoGame/Content/Scripts/gym_brock.lua
    - tests/SDK.Core.Tests/SaveSystemTests.cs
    - tests/SDK.MonoGame.Tests/DialogueBoxTests.cs
    - tests/SDK.Scripting.Tests/ProdScriptTests.cs
  modified:
    - src/SDK.MonoGame/SDK.MonoGame.csproj
    - src/SDK.MonoGame/Program.cs
    - src/SDK.MonoGame/Game1.cs
    - src/SDK.MonoGame/HeadlessRunner.cs (cherry-pick 98c3299)
    - src/SDK.MonoGame/Rendering/RenderPipeline.cs (cherry-pick 98c3299)
    - src/SDK.MonoGame/World/WorldSystem.cs (cherry-pick 98c3299)

key-decisions:
  - "D-06 résolu définitivement : SDK.MonoGame.csproj PEUT référencer SDK.Scripting — contrainte = Game1.cs n'utilise jamais LuaScriptEngine directement"
  - "Func<IScriptEngine> factory (pas singleton) — engine frais par interaction NPC, évite pollution d'état entre scripts"
  - "SaveSystem dans SDK.Core.Services — System.Text.Json est BCL .NET 10, zéro NuGet supplémentaire"
  - "DialogueBox.Draw() stub no-op — SpriteFont/MGCB compilation déférée Phase 7 DX"
  - "gym_brock.lua contenu minimal : badges:AwardBadge('boulder') uniquement — dialogue: API non câblée au NpcInteractionRunner dans ce plan"

patterns-established:
  - "ISaveSystem contrat minimal : Save(GameState, path) + Load(path) → GameState? — extensible Phase 5+ pour IGameClock.GameElapsed"
  - "Composition root (Program.cs) seul lieu où types concrets de SDK.Scripting sont référencés"

duration: ~15min
started: 2026-06-05T22:35:00Z
completed: 2026-06-05T22:50:00Z
---

# Phase 4 Plan 03 : SaveSystem JSON + DialogueBox + Game1 Wiring + CodeQL cherry-pick Summary

**SaveSystem implémente ISaveSystem via System.Text.Json BCL (zéro NuGet), Game1 câblé ISaveSystem + Func<IScriptEngine> (D-06 compliant), cherry-pick 98c3299 CodeQL fixes appliqué, DialogueBox stub + gym_brock.lua prod — 97/97 tests. Phase 4 complète.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~15 min |
| Started | 2026-06-05T22:35:00Z |
| Completed | 2026-06-05T22:50:00Z |
| Tasks | 3 complétées |
| Files modifiés | 12 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: SaveSystem round-trip JSON | Pass | `Save_Load_RoundTrip_PreservesAllFields` : PlayerName + PlaytimeSeconds + badge_boulder préservés |
| AC-2: Load retourne null si fichier absent | Pass | `Load_ReturnsNull_WhenFileNotFound` : pas d'exception |
| AC-3: D-06 — Game1.cs sans SDK.Scripting direct | Pass | `grep LuaScriptEngine Game1.cs` → vide ✅ |
| AC-4: DialogueBox état machine Open/Close | Pass | `Open_SetsIsOpenTrue_AndCurrentText` + `Close_SetsIsOpenFalse` |
| AC-5: gym_brock.lua via NpcInteractionRunner | Pass | `GymBrockScript_AwardsBoulderBadge` : GetFlag<bool>("badge_boulder") == true |
| AC-6: Suite complète >= 96 tests | Pass | 97 tests, 0 failures, 0 régressions |

## Accomplishments

- SaveSystem serialise/désérialise GameState complet (PlayerName + PlaytimeSeconds + Flags D-12) sans perte — zéro NuGet ajouté à SDK.Core.csproj
- Cherry-pick 98c3299 appliqué proprement : `_graphics` readonly, HeadlessRunner clock supprimé, RenderPipeline readonly + catch spécifiques, WorldSystem LINQ Any
- Game1.cs câblé `_saveSystem` + `_scriptEngineFactory` via DI — D-06 vérifié, LuaScriptEngine invisible dans Game1.cs
- Program.cs enregistre `Func<IScriptEngine>` factory et `ISaveSystem` — composition root unique pour les types concrets SDK.Scripting
- DialogueBox stub état machine testée (Open/Close/IsOpen/CurrentText)
- gym_brock.lua attribue badge boulder via NpcInteractionRunner — SCRIPT-01→03 satisfaits
- 89 → 97 tests (+8), 0 régressions

## Task Commits

| Task | Commit | Description |
|------|--------|-------------|
| Cherry-pick | `4cbf6fa` | fix(code-quality): adresse suggestions CodeQL PR #7 |
| T1+T2+T3 | `ff38a05` | feat(scripting): Plan 04-03 — SaveSystem JSON + DialogueBox + Game1 wiring |

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Core/Services/SaveSystem.cs` | Créé | ISaveSystem via System.Text.Json BCL |
| `src/SDK.MonoGame/UI/DialogueBox.cs` | Créé | Stub état machine Open/Close, Draw() no-op |
| `src/SDK.MonoGame/Content/Scripts/gym_brock.lua` | Créé | Script prod Gen1 boulder badge |
| `src/SDK.MonoGame/SDK.MonoGame.csproj` | Modifié | ProjectReference SDK.Scripting ajouté (D-06) |
| `src/SDK.MonoGame/Program.cs` | Modifié | Func<IScriptEngine> factory + ISaveSystem enregistrés |
| `src/SDK.MonoGame/Game1.cs` | Modifié | _saveSystem + _scriptEngineFactory champs + cherry-pick |
| `src/SDK.MonoGame/HeadlessRunner.cs` | Modifié | Cherry-pick : var clock inutilisé supprimé |
| `src/SDK.MonoGame/Rendering/RenderPipeline.cs` | Modifié | Cherry-pick : readonly fields + catch spécifiques |
| `src/SDK.MonoGame/World/WorldSystem.cs` | Modifié | Cherry-pick : foreach → LINQ Any |
| `tests/SDK.Core.Tests/SaveSystemTests.cs` | Créé | 4 tests : round-trip, null, overwrite, D-12 flags |
| `tests/SDK.MonoGame.Tests/DialogueBoxTests.cs` | Créé | 2 tests : Open state + Close state |
| `tests/SDK.Scripting.Tests/ProdScriptTests.cs` | Créé | 2 tests : badge awarded + idempotent |

## Decisions Made

| Décision | Rationale | Impact |
|----------|-----------|--------|
| D-06 final : csproj peut référencer SDK.Scripting | CLAUDE.md arch confirme `SDK.MonoGame ← ... + SDK.Scripting (via Func factory)` — contrainte = code Game1.cs uniquement | Composition root propre, DI standard |
| Func<IScriptEngine> factory (AddSingleton) | Engine frais par interaction NPC — pas de pollution d'état entre scripts successifs | Plan 05+ peut créer engines parallèles |
| SaveSystem dans SDK.Core.Services | System.Text.Json BCL — SDK.Core.csproj reste vide de NuGet | D-01 + D-10 respectés |

## Deviations from Plan

### Summary

| Type | Count | Impact |
|------|-------|--------|
| Auto-fixed | 0 | — |
| Scope additions | 0 | — |
| Deferred | 0 | — |

**Total impact :** Plan exécuté exactement comme spécifié.

## Issues Encountered

| Issue | Résolution |
|-------|------------|
| Build cache MSB3492 au premier build | Second build propre — artefact transitoire MSBuild, non bloquant |

## Phase 4 Completion

**Phase 4 — Scripting + Progression : COMPLÈTE ✅**

| Requirement | Status |
|-------------|--------|
| SCRIPT-01 : MoonSharp SoftSandbox + IScriptEngine | ✅ Plan 04-01 |
| SCRIPT-02 : GameState + Flags D-12 | ✅ Plan 04-01 |
| SCRIPT-03 : Save/Load GameState + badge Lua prod | ✅ Plan 04-03 |

## Next Phase Readiness

**Prêt pour Phase 5 (Plugins + Characters) :**
- SaveSystem disponible — Phase 5 peut persister `IGameClock.GameElapsed` via `SetGameTime`
- Func<IScriptEngine> factory établie — Phase 5 peut créer engines par interaction NPC parallèle
- DialogueBox stub — Phase 5 peut câbler au NpcInteractionRunner avec DialogApi
- 97 tests verts — base solide pour Phase 5

**Concerns :**
- DialogueBox.Draw() stub — SpriteFont absent jusqu'à Phase 7 DX (MGCB compilation)
- SaveSystem ne persiste pas encore `GameElapsed` (IGameClock) — déféré Phase 5+
- F5/F9 key bindings save/load non câblés — déféré Phase 5

**Blockers :**
- Aucun

---
*Phase: 04-scripting-progression, Plan: 03*
*Complété: 2026-06-05*
