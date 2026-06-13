---
sidebar_position: 1
---

# API avancées

Points d'extension de PokemonSDK — moteurs de narration, génération de Fakemons, et pipelines d'assets.

| API | Package | Description |
|-----|---------|-------------|
| [INarrationPlugin](./narration-plugin.md) | SDK.Plugins.TTS | Interface pour backends TTS personnalisés |
| [FakemonAssemblyPipeline](./fakemon-pipeline.md) | SDK.Tools | Assembler des sprites Fakemon composites |
| [FakemonPartsCatalog](./fakemon-catalog.md) | SDK.Tools | Scanner et filtrer les images de parties Fakemon |

Ces APIs respectent le contrat D-21 — elles constituent la **surface publique stable** documentée ici. Les classes d'implémentation internes ne sont pas couvertes.
