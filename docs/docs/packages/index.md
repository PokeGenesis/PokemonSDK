---
sidebar_position: 1
---

# Packages overview

PokemonSDK is split into 8 focused NuGet packages. Add only what your project needs.

| Package | NuGet ID | Key dependency |
|---------|----------|----------------|
| [Core](./core.md) | `PokeForge.SDK.Core` | none |
| [Data](./data.md) | `PokeForge.SDK.Data` | EF Core 10 + SQLite |
| [Battle](./battle.md) | `PokeForge.SDK.Battle` | Core |
| [Scripting](./scripting.md) | `PokeForge.SDK.Scripting` | Core + MoonSharp 2.0 |
| [MonoGame](./monogame.md) | `PokeForge.SDK.MonoGame` | Core + MonoGame.DesktopGL |
| [Tools](./tools.md) | `PokeForge.SDK.Tools` | Core + Data + SixLabors.ImageSharp |
| [Plugins](./plugins.md) | `PokeForge.SDK.Plugins` | Core + Battle |
| [Plugins.TTS](./plugins-tts.md) | `PokeForge.SDK.Plugins.TTS` | Core |

## Dependency rules

```
SDK.Core      ← zero external NuGet dependencies
SDK.Data      ← Core + EF Core 10
SDK.Battle    ← Core only
SDK.Scripting ← Core + MoonSharp 2.0
SDK.Plugins.* ← Core + Battle
SDK.Tools     ← Core + Data + SixLabors.ImageSharp
SDK.MonoGame  ← Core + MonoGame + Battle + Scripting (via Func factory)
```

`SDK.Core` is the only package with zero external dependencies by design — it can be referenced from any project without pulling in a transitive dependency chain.
