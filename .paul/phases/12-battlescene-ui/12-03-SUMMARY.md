---
phase: 12-battlescene-ui
plan: 03
type: summary
completed: 2026-06-14
---

# Summary: Plan 12-03 — HpBar Widget + BattleScene State Machine

## What Was Built

### New Files

- `src/SDK.MonoGame/UI/HpBar.cs`: Widget HP GBA-style. 3 couleurs (vert >50%, jaune 20-50%, rouge <20%). Fond DarkGray. Label textuel optionnel. Dispose() sur Texture2D 1×1.
- `src/SDK.MonoGame/Scenes/BattleScene.cs`: Machine à états (enum BattlePhase: Init, SelectMove, Running, BattleEnd). LoadBattle(BattleState), SetPlayerMove(BattleMove), ExecuteTurn() via IBattleEngine.RunTurn(). Draw() : fond noir + sprites placeholder 96×96 + HpBar adversaire (pos 160,10) + HpBar joueur (pos 20,130) + overlay "BATTLE END" en BattlePhase.BattleEnd.

### Modified Files

- `src/SDK.MonoGame/Program.cs`: Ajout `using SDK.Battle`, `using SDK.MonoGame.Scenes`. Enregistrement DI `services.AddSingleton<IBattleEngine, BattleEngine>()` + `services.AddSingleton<BattleScene>()`.
- `src/SDK.MonoGame/Game1.cs`: Champ `_battleScene BattleScene`. Résolution DI dans `Initialize()`. Appel `_battleScene.Initialize(GraphicsDevice, _font)` dans `LoadContent()`.

## Decisions Made

**Two-phase initialization (deviation de la spec originale):** BattleScene constructor prend seulement `IBattleEngine` (DI-friendly). Un `Initialize(GraphicsDevice, SpriteFont?)` appelé depuis `Game1.LoadContent()` crée HpBar et Texture2D pixel. Raison: GraphicsDevice n'est pas disponible au moment du `BuildServiceProvider()` — seulement après `Game1.Initialize()`. Pattern cohérent avec les contraintes MonoGame.

## Acceptance Criteria Results

| AC | Result |
|----|--------|
| AC-1: HpBar 3 couleurs 480×270 | PASS |
| AC-2: BattleScene state machine Init→SelectMove→Running→BattleEnd | PASS |
| AC-3: Draw() HP bars + sprites placeholder 96×96 | PASS |
| AC-4: BattleScene singleton DI, `GetRequiredService<BattleScene>()` fonctionne | PASS (via two-phase init, résolu dans Game1.Initialize()) |

## Verification

- `dotnet build src/SDK.MonoGame/SDK.MonoGame.csproj`: 0 erreurs, 0 warnings
- `dotnet test tests/SDK.MonoGame.Tests/`: 11/11 verts
- `dotnet build PokemonSDK.slnx`: 0 erreurs, 0 warnings

## Deferred Issues

- BattleScene.Draw() ne rend pas encore MoveMenu ni StatusIcon (Plan 12-04)
- Retour à WorldScene depuis BattleScene: Plan 12-05
- Sprites PNG réels D-16: Phase 17
