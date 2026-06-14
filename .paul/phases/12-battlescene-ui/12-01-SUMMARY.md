---
phase: 12-battlescene-ui
plan: 01
status: complete
completed: 2026-06-14
---

# Summary: Plan 12-01 — StatusConditionId + RunTurn extraction

## What was built

- `src/SDK.Core/Enums/StatusConditionId.cs` (new): enum None=0, Sleep=1, Freeze=2, Burn=3, Poison=4, Paralysis=5
- `src/SDK.Core/ValueObjects/BattlePokemon.cs`: added `StatusConditionId? Status = null` as 14e paramètre positionnel avec default — backward-compatible
- `src/SDK.Core/Interfaces/IBattleEngine.cs`: ajout `RunTurn(BattleState, BattleMove, BattleMove)` + `SelectOpponentMove(BattleState)`
- `src/SDK.Battle/BattleEngine.cs`: `RunTurn()` extrait de `RunBattle()` + `SelectOpponentMove()` délègue à `_opponentStrategy`
- `tests/SDK.Battle.Tests/BattleEngineTests.cs`: 6 nouveaux tests RunTurn

## Acceptance criteria results

- AC-1: StatusConditionId enum créé — 5 valeurs + None ✅
- AC-2: BattlePokemon.Status nullable, default null, backward-compatible ✅
- AC-3: IBattleEngine étendu — RunTurn + SelectOpponentMove ✅
- AC-4: RunTurn extrait, RunBattle délègue via RunTurn + SelectOpponentMove ✅
- AC-5: 6 tests passent (47 total SDK.Battle.Tests) ✅

## Tests added

1. `RunTurn_ReducesOpponentHp_WhenPlayerMoveHits`
2. `RunTurn_IncrementsTurnCounter`
3. `RunTurn_ReturnsNewState_OriginalTurnUnchanged`
4. `RunTurn_PlayerGoesFirst_WhenFaster_OpponentNeverAttacks`
5. `RunTurn_OpponentGoesFirst_WhenFaster_PlayerNeverAttacks`
6. `SelectOpponentMove_DelegatesToOpponentStrategy`

## Decisions made

- `Status` ajouté en DERNIER param positionnel avec default null: tous les appels existants `new BattlePokemon(...)` compilent sans modification
- `SelectOpponentMove` sur IBattleEngine (pas seulement BattleEngine): BattleScene peut l'appeler via l'interface sans accès direct à IDifficultyMode

## Build result

`dotnet build PokemonSDK.slnx` — 0 erreurs, 0 warnings
`dotnet test tests/SDK.Battle.Tests/` — 47/47 passés
