---
sidebar_position: 6
---

# Rendu HD : 480x270 vers 1080p

Ce guide montre comment configurer `RenderPipeline`, implémenter la boucle Draw en 3 passes, et utiliser le shader de mise à l'échelle xBR.

## Installer

```bash
dotnet add package PokeForge.SDK.MonoGame
```

## Contrat de résolution

PokemonSDK effectue son rendu interne à **480x270** et monte à l'échelle vers **1920x1080** via le shader xBR. Cette résolution est fixe et ne peut pas être modifiée :

| Étape | Résolution |
|-------|-----------|
| Rendu du monde | 480x270 |
| Mise à l'échelle xBR (x4) | 1920x1080 |
| Superposition UI | 1920x1080 (natif) |

Le canvas 480x270 donne l'aspect pixel-art des jeux Pokémon classiques, tandis que la passe xBR produit des bords lisses et sans artefacts à toute taille d'écran.

## Configuration de RenderPipeline

Créez `RenderPipeline` dans `LoadContent()` et stockez-le sur votre `Game1` :

```csharp
using PokeForge.SDK.MonoGame;

protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _pipeline = new RenderPipeline(GraphicsDevice);
    _pipeline.LoadShaders(Content);
}
```

Définissez la taille de la fenêtre à 1920x1080 (ou laissez l'utilisateur basculer en plein écran) :

```csharp
_graphics.PreferredBackBufferWidth  = 1920;
_graphics.PreferredBackBufferHeight = 1080;
_graphics.ApplyChanges();
```

## Draw() en 3 passes

La boucle Draw comporte trois passes distinctes :

```csharp
protected override void Draw(GameTime gameTime)
{
    // Passe 1 : rendu du monde à 480x270 dans un RenderTarget2D
    GraphicsDevice.SetRenderTarget(_pipeline.WorldTarget);
    GraphicsDevice.Clear(Color.Black);
    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    _worldRenderer.Draw(_spriteBatch, _gameState);
    _spriteBatch.End();

    // Passe 2 : appliquer le shader de mise à l'échelle xBR (480x270 -> 1920x1080)
    GraphicsDevice.SetRenderTarget(null);
    _pipeline.ApplyXbr(_spriteBatch);

    // Passe 3 : dessiner l'UI à 1920x1080 natif (sans mise à l'échelle)
    _spriteBatch.Begin();
    _uiRenderer.Draw(_spriteBatch, _gameState);
    _spriteBatch.End();
}
```

Gardez les sprites du monde sur la Passe 1 et les éléments UI sur la Passe 3. Les mélanger casse la mise à l'échelle.

## Teinte jour/nuit

Le `DayNightShader` teinte le frame mis à l'échelle en fonction de l'heure en jeu :

```csharp
// Configurer les plages horaires et les couleurs de teinte
var dayNightConfig = new DayNightConfig
{
    DawnHour    = 6,
    DuskHour    = 20,
    NightColor  = new Color(30, 30, 80),   // bleu profond
    DawnColor   = new Color(255, 180, 120), // orange chaud
};

_pipeline.SetDayNightConfig(dayNightConfig);
```

Passez l'heure actuelle depuis votre horloge de jeu dans `Draw()` :

```csharp
_pipeline.SetTimeOfDay(_gameClock.Hour);
_pipeline.ApplyXbr(_spriteBatch);  // applique la teinte lors de cet appel
```

## Repli de shader

Les shaders sont compilés via MonoGame Content Builder (MGCB). Si le fichier `.xnb` compilé est absent, `RenderPipeline` revient à `SamplerState.PointClamp` sans shader de mise à l'échelle, ce qui produit tout de même un pixel-art net aux échelles entières.

Pour compiler les shaders :

```bash
dotnet mgcb-editor  # ouvrir Content/Content.mgcb, tout compiler
```
