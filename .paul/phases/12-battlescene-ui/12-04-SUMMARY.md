---
phase: 12-battlescene-ui
plan: 04
type: summary
completed: 2026-06-14
---

# Summary: Plan 12-04 — StatusIcon + MoveMenu + BattleScene Integration

## What Was Built

### New Files

- `src/SDK.MonoGame/UI/StatusIcon.cs`: Indicateur visuel de statut. 5 conditions (SLP=CornflowerBlue, FRZ=Cyan, BRN=OrangeRed, PSN=MediumPurple, PRZ=Yellow). Carré 24×10 pixels + label DrawString scale 0.5f. None/null = no-op. Dispose() sur Texture2D pixel.
- `src/SDK.MonoGame/UI/MoveMenu.cs`: Menu 4 moves navigable. Up/Down wraparound, Z confirme → `SelectedMove` signal. Fond coloré par TypeId (18 types mappés, fallback Gray). Curseur `▶` DrawString scale 0.6f. `ResetSelection()` remet SelectedMove à null. Constructor prend `IReadOnlyList<BattleMove>` + `GraphicsDevice`.

### Modified Files

- `src/SDK.MonoGame/Scenes/BattleScene.cs`: Ajout `_statusIcon StatusIcon?` + `_moveMenu MoveMenu?` + `_graphicsDevice GraphicsDevice?`. `Initialize()` crée StatusIcon + stocke GraphicsDevice. `LoadBattle()` crée MoveMenu avec les moves du joueur. `Update()` branche SelectMove: `_moveMenu.Update(Keyboard.GetState())`, SelectedMove → `SetPlayerMove` + `ResetSelection`. `Draw()`: StatusIcon pour player (20,150) et opponent (160,20), MoveMenu à (5,200) en phase SelectMove.

## Decisions Made

**MoveMenu(IReadOnlyList\<BattleMove\>, GraphicsDevice) — déviation de spec:** Le plan spec montrait `MoveMenu(moves)` uniquement. Ajout de `GraphicsDevice` nécessaire pour créer la Texture2D pixel utilisée dans Draw(). Pattern identique à HpBar et StatusIcon. Déviation mineure, correcte.

## Acceptance Criteria Results

| AC | Result |
|----|--------|
| AC-1: StatusIcon 5 couleurs + None no-op | PASS |
| AC-2: MoveMenu 4 moves, couleurs de type, curseur ▶ | PASS |
| AC-3: Up/Down/Z navigation, wraparound, SelectedMove signal | PASS |
| AC-4: BattleScene intègre MoveMenu + StatusIcon dans Update/Draw | PASS |

## Verification

- `dotnet build src/SDK.MonoGame/SDK.MonoGame.csproj`: 0 erreurs, 0 warnings
- `dotnet test tests/SDK.MonoGame.Tests/`: 11/11 verts
- `dotnet build PokemonSDK.slnx`: 0 erreurs, 0 warnings

## Deferred Issues

- Wipe transition + BattleEndOverlay: Plan 12-05
- PP moves (épuisement): hors scope BTLUI-01
- Sons SFX sélection: Phase 18 SFX-01
