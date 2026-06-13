---
sidebar_position: 6
---

# PokeForge.SDK.MonoGame

MonoGame DesktopGL runtime — pixel-art HD pipeline, Tiled world maps, day/night shader.

```bash
dotnet add package PokeForge.SDK.MonoGame
```

> **Platform requirement**: DesktopGL only. `MonoGame.Framework.WindowsDX` is not supported.

## HD render pipeline

All rendering targets an internal canvas of **480 × 270** (16:9 at 1× pixel-art scale). The final draw pass upscales to the OS window via the **xBR ×4** shader, producing crisp pixel art at 1920 × 1080 with no blurring.

```
Game canvas  →  RenderTarget 480×270  →  xBR ×4  →  OS window 1920×1080
```

The internal resolution is fixed and must not be changed.

## Scene setup

Derive from `SdkGame` and register your scenes:

```csharp
public class MyGame : SdkGame
{
    protected override void RegisterScenes(SceneManager scenes)
    {
        scenes.Register<WorldScene>();
        scenes.Register<BattleScene>();
        scenes.Register<DialogScene>();
    }
}
```

Switch scenes from anywhere:

```csharp
SceneManager.Push<BattleScene>(new BattleArgs(playerTeam, rivalTeam));
SceneManager.Pop(); // returns to WorldScene
```

## WorldSystem + Tiled maps

Maps are authored in [Tiled](https://www.mapeditor.org/) and imported as `.tmx` files:

```csharp
public class WorldScene : Scene
{
    protected override void Load()
    {
        WorldSystem.LoadMap("maps/pallet_town.tmx");
    }
}
```

`WorldSystem` handles:
- Tile rendering (multi-layer)
- Collision layer parsing
- Object layer (NPC spawns, warp triggers, item pickups)
- Camera follow with clamped bounds

## Day/Night system

```csharp
// Drive time from GameState (0–23)
DayNightSystem.SetTime(gameState.InGameHour);
```

The DayNight shader blends overlay tints across four phases:

| Phase | Hours | Tint |
|-------|-------|------|
| Dawn | 5–7 | warm orange |
| Day | 8–17 | neutral |
| Dusk | 18–20 | amber |
| Night | 21–4 | deep blue |

## Scripting integration

`SDK.MonoGame` does **not** directly reference `SDK.Scripting` (avoids circular dependency). Script execution is injected as a `Func<string, Task>` at the composition root:

```csharp
// In Game1 / DI setup
services.AddSingleton<Func<string, Task>>(sp =>
    code => sp.GetRequiredService<LuaScriptEngine>().ExecuteAsync(code));
```

## Draw passes

Each frame runs three ordered passes:

1. **World pass** — tilemaps + sprites → RenderTarget 480×270
2. **UI pass** — HUD, dialog boxes → same RenderTarget (no scaling artifacts)
3. **Upscale pass** — xBR ×4 shader → OS backbuffer
