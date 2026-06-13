---
sidebar_position: 6
---

# HD Rendering: 480x270 to 1080p

This guide shows how to configure `RenderPipeline`, implement the 3-pass Draw loop, and use the xBR upscale shader.

## Install

```bash
dotnet add package PokeForge.SDK.MonoGame
```

## Resolution Contract

PokemonSDK renders internally at **480x270** and upscales to **1920x1080** using the xBR shader. This resolution is fixed and cannot be changed:

| Stage | Resolution |
|-------|-----------|
| World render | 480x270 |
| xBR upscale (x4) | 1920x1080 |
| UI overlay | 1920x1080 (native) |

The 480x270 canvas gives you the pixel-art look of classic Pokémon games while the xBR pass produces smooth, artifact-free edges at any monitor size.

## RenderPipeline Setup

Create `RenderPipeline` in `LoadContent()` and store it on your `Game1`:

```csharp
using PokeForge.SDK.MonoGame;

protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _pipeline = new RenderPipeline(GraphicsDevice);
    _pipeline.LoadShaders(Content);
}
```

Set the window size to 1920x1080 (or let the user toggle fullscreen):

```csharp
_graphics.PreferredBackBufferWidth  = 1920;
_graphics.PreferredBackBufferHeight = 1080;
_graphics.ApplyChanges();
```

## 3-Pass Draw()

The Draw loop has three distinct passes:

```csharp
protected override void Draw(GameTime gameTime)
{
    // Pass 1: render world at 480x270 into a RenderTarget2D
    GraphicsDevice.SetRenderTarget(_pipeline.WorldTarget);
    GraphicsDevice.Clear(Color.Black);
    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    _worldRenderer.Draw(_spriteBatch, _gameState);
    _spriteBatch.End();

    // Pass 2: apply xBR upscale shader (480x270 -> 1920x1080)
    GraphicsDevice.SetRenderTarget(null);
    _pipeline.ApplyXbr(_spriteBatch);

    // Pass 3: draw UI at native 1920x1080 (no scaling)
    _spriteBatch.Begin();
    _uiRenderer.Draw(_spriteBatch, _gameState);
    _spriteBatch.End();
}
```

Keep world sprites on Pass 1 and UI elements on Pass 3. Mixing them breaks the scaling.

## Day/Night Tint

The `DayNightShader` tints the upscaled frame based on the current in-game hour:

```csharp
// Configure time ranges and tint colors
var dayNightConfig = new DayNightConfig
{
    DawnHour    = 6,
    DuskHour    = 20,
    NightColor  = new Color(30, 30, 80),   // deep blue
    DawnColor   = new Color(255, 180, 120), // warm orange
};

_pipeline.SetDayNightConfig(dayNightConfig);
```

Pass the current hour from your game clock in `Draw()`:

```csharp
_pipeline.SetTimeOfDay(_gameClock.Hour);
_pipeline.ApplyXbr(_spriteBatch);  // applies tint during this call
```

## Shader Fallback

Shaders are compiled via MonoGame Content Builder (MGCB). If the compiled `.xnb` is missing, `RenderPipeline` falls back to `SamplerState.PointClamp` with no upscale shader, which still produces sharp pixel art at integer scales.

To compile shaders:

```bash
dotnet mgcb-editor  # open Content/Content.mgcb, build all
```
