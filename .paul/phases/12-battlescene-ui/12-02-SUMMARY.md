---
phase: 12-battlescene-ui
plan: 02
status: complete
completed: 2026-06-14
---

# Summary: Plan 12-02 — IGameScene + WorldScene + Game1 refactor

## What was built

- `src/SDK.MonoGame/Scenes/IGameScene.cs` (new): interface `Update(GameTime)` + `Draw(SpriteBatch, GameTime)`
- `src/SDK.MonoGame/Scenes/WorldScene.cs` (new): implémente IGameScene, wrappe WorldSystem + PlayerSystem
- `src/SDK.MonoGame/Game1.cs`: remplace `_world`/`_player` directs par `_currentScene IGameScene` + `_pendingScene IGameScene?`; ajoute `SwitchToScene(IGameScene)`

## Acceptance criteria results

- AC-1: IGameScene interface créée — 2 méthodes ✅
- AC-2: WorldScene implémente IGameScene — Update/Draw délèguent à world/player ✅
- AC-3: Game1 utilise `_currentScene.Update()` + `_currentScene.Draw()` ✅
- AC-4: `_pendingScene` buffer — swap sûr en début de Update() ✅
- AC-5: `SwitchToScene(IGameScene)` public — disponible pour Plans 12-03/12-05 ✅
- AC-6: HeadlessRunner (ne passe pas par Game1) — 11/11 smoke tests verts ✅

## Key design decisions

- `_pendingScene` swappé en DÉBUT de Update(), avant `_currentScene.Update()`: évite les race conditions mid-frame
- `keyboard.Update()` reste dans Game1 (infra), pas dans WorldScene: cohérent pour BattleScene aussi
- WorldScene construit via `new WorldScene(world, player)` dans Initialize() — pas de DI registration (Program.cs inchangé pour ce plan)
- Game1 hérite `GraphicsDevice` public du base Game: pas de propriété wrapper nécessaire

## Build result

`dotnet build PokemonSDK.slnx` — 0 erreurs, 0 warnings
`dotnet test tests/SDK.MonoGame.Tests/` — 11/11 smoke tests passés
