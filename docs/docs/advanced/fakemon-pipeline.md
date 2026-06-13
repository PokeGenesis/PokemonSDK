---
sidebar_position: 3
---

# FakemonAssemblyPipeline

Assembles composite Fakemon sprites from layered part images. Produces all five views required by the D-16 convention.

## Constructor

```csharp
public FakemonAssemblyPipeline(FakemonPartsCatalog catalog)
```

## AssembleAsync

```csharp
public Task<AssemblyResult> AssembleAsync(AssemblyOptions options);
```

### AssemblyOptions

```csharp
public record AssemblyOptions
{
    /// Fakemon identifier (kebab-case, e.g. "dragon-electric")
    public required string Identifier { get; init; }

    /// Part key for the body layer (matched against catalog)
    public required string Body { get; init; }

    /// Part key for the head layer
    public required string Head { get; init; }

    /// Optional part key for the tail layer
    public string? Tail { get; init; }

    /// Optional part key for an accessory overlay
    public string? Accessory { get; init; }

    /// Target directory for output sprites
    public required string OutputDirectory { get; init; }

    /// Views to generate (default: all five)
    public IReadOnlyList<string> Views { get; init; } =
        new[] { "front", "back", "overworld", "portrait", "icon" };
}
```

### AssemblyResult

```csharp
public record AssemblyResult
{
    public bool Success { get; init; }
    public IReadOnlyList<string> OutputPaths { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}
```

## Example

```csharp
var catalog = new FakemonPartsCatalog("assets/fakemon-parts/");
var pipeline = new FakemonAssemblyPipeline(catalog);

var result = await pipeline.AssembleAsync(new AssemblyOptions
{
    Identifier   = "dragon-electric",
    Body         = "dragon_base",
    Head         = "electric_head",
    Tail         = "spiked_tail",
    OutputDirectory = "assets/sprites/fakemons/",
    Views        = new[] { "front", "back", "icon" }
});

if (!result.Success)
    foreach (var err in result.Errors)
        Console.Error.WriteLine(err);
```

## Output naming

Output files follow the D-16 Fakemon convention:

```
fk_{identifier}_{view}.png
```

The `icon` view is always resized to **32×32** pixels regardless of source part dimensions.
