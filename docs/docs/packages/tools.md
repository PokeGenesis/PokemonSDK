---
sidebar_position: 7
---

# PokeForge.SDK.Tools

Developer tooling sprite validation, atlas packing, database sync, and the Fakemon assembly pipeline. Runs headless (no MonoGame dependency) so it can execute in CI.

```bash
dotnet add package PokeForge.SDK.Tools
```

## SpriteValidator

Validates sprite filenames against the D-16 naming convention before they reach the game.

**Convention**: `{dexid5}_{identifier}_{view}.png` for real species, `fk_{identifier}_{view}.png` for Fakemons.

Valid views: `front`, `back`, `overworld`, `portrait`, `icon` (32×32).

```csharp
var validator = new SpriteValidator();
var results = validator.ValidateDirectory("assets/sprites/");

foreach (var error in results.Errors)
    Console.WriteLine($"{error.File}: {error.Message}");
```

Regex enforced:

```
^(\d{5}_[a-z0-9-]+|fk_[a-z0-9-]+)_(front|back|overworld|portrait|icon)\.png$
```

## AtlasPacker

Packs individual sprite PNGs into a single texture atlas (lossless PNG). Uses `SixLabors.ImageSharp`.

```csharp
var packer = new AtlasPacker();
var atlas = await packer.PackAsync(
    sourceDir: "assets/sprites/",
    outputPath: "assets/atlas.png",
    metaPath: "assets/atlas.json");

// atlas.json contains UV coordinates per sprite
```

## SqliteSyncer

Syncs sprite metadata (path, dimensions, atlas UV) from the atlas JSON into the SQLite database.

```csharp
var syncer = new SqliteSyncer(db);
await syncer.SyncAtlasAsync("assets/atlas.json");
```

## FakemonAssemblyPipeline

Assembles composite Fakemon sprites by layering body-part images. See [Advanced APIs → FakemonAssemblyPipeline](../advanced/fakemon-pipeline) for the full API reference.

```csharp
var catalog = new FakemonPartsCatalog("assets/fakemon-parts/");
var pipeline = new FakemonAssemblyPipeline(catalog);
var result = await pipeline.AssembleAsync(new AssemblyOptions
{
    Identifier = "dragon-electric",
    Body = "dragon_base",
    Head = "electric_head",
    OutputDirectory = "assets/sprites/fakemons/"
});
```

## FakemonPartsCatalog

Scans a parts directory and provides filtering. See [Advanced APIs → FakemonPartsCatalog](../advanced/fakemon-catalog).
