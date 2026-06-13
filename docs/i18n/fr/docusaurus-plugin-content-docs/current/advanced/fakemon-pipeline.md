---
sidebar_position: 3
---

# FakemonAssemblyPipeline

Assemble des sprites Fakemon composites à partir d'images de parties superposées. Produit les cinq vues requises par la convention D-16.

## Constructeur

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
    /// Identifiant Fakemon (kebab-case, ex. "dragon-electrique")
    public required string Identifier { get; init; }

    /// Clé de partie pour la couche corps (correspondance dans le catalog)
    public required string Body { get; init; }

    /// Clé de partie pour la couche tête
    public required string Head { get; init; }

    /// Clé de partie optionnelle pour la couche queue
    public string? Tail { get; init; }

    /// Clé de partie optionnelle pour un accessoire superposé
    public string? Accessory { get; init; }

    /// Répertoire de destination pour les sprites de sortie
    public required string OutputDirectory { get; init; }

    /// Vues à générer (défaut : les cinq)
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

## Exemple

```csharp
var catalog = new FakemonPartsCatalog("assets/fakemon-parts/");
var pipeline = new FakemonAssemblyPipeline(catalog);

var result = await pipeline.AssembleAsync(new AssemblyOptions
{
    Identifier      = "dragon-electrique",
    Body            = "dragon_base",
    Head            = "electric_head",
    Tail            = "spiked_tail",
    OutputDirectory = "assets/sprites/fakemons/",
    Views           = new[] { "front", "back", "icon" }
});

if (!result.Success)
    foreach (var err in result.Errors)
        Console.Error.WriteLine(err);
```

## Nommage des fichiers de sortie

Les fichiers de sortie suivent la convention Fakemon D-16 :

```
fk_{identifiant}_{vue}.png
```

La vue `icon` est toujours redimensionnée à **32×32** pixels quelle que soit la dimension des parties sources.
