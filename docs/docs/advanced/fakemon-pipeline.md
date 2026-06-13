---
sidebar_position: 3
---

# FakemonAssemblyPipeline

Pipeline for assembling composite Fakemon sprites from part images.

```csharp
var pipeline = new FakemonAssemblyPipeline(catalog, ctx);
var result = await pipeline.AssembleAsync(options);
```
