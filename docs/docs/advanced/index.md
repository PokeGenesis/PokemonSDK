---
sidebar_position: 1
---

# Advanced APIs

Extension points for PokemonSDK narration engines, custom Fakemon generation, and asset pipelines.

| API                                              | Package         | Description                         |
| ------------------------------------------------ | --------------- | ----------------------------------- |
| [INarrationPlugin](./narration-plugin.md)        | SDK.Plugins.TTS | Interface for custom TTS backends   |
| [FakemonAssemblyPipeline](./fakemon-pipeline.md) | SDK.Tools       | Assemble composite Fakemon sprites  |
| [FakemonPartsCatalog](./fakemon-catalog.md)      | SDK.Tools       | Scan and filter Fakemon part images |

These APIs follow the D-21 contract: only the **stable, public surface** is documented here. Internal implementation classes are not covered.
