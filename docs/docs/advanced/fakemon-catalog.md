---
sidebar_position: 4
---

# FakemonPartsCatalog

Scans a directory for Fakemon part images and provides lookup and filtering for the assembly pipeline.

## Constructor

```csharp
public FakemonPartsCatalog(string partsDirectory)
```

`partsDirectory` should contain sub-folders per category (`body/`, `head/`, `tail/`, `accessory/`).

## Scan

```csharp
public IReadOnlyList<FakemonPart> Scan();
```

Returns all `.png` files discovered in the parts directory, parsed into `FakemonPart` records.

### FakemonPart

```csharp
public record FakemonPart
{
    public string Key { get; init; }         // filename without extension
    public string Category { get; init; }    // "body" | "head" | "tail" | "accessory"
    public string AbsolutePath { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
}
```

## GetPart

```csharp
public FakemonPart? GetPart(string key, string category);
```

Looks up a specific part by key and category. Returns `null` if not found.

```csharp
var head = catalog.GetPart("electric_head", "head");
if (head is null)
    throw new InvalidOperationException("Part 'electric_head' not found");
```

## Filter

```csharp
public IReadOnlyList<FakemonPart> Filter(string category);
```

Returns all parts in a given category.

```csharp
var allHeads = catalog.Filter("head");
Console.WriteLine($"{allHeads.Count} head parts available");
```

## Example

```csharp
var catalog = new FakemonPartsCatalog("assets/fakemon-parts/");
var parts = catalog.Scan();

// Group by category
var byCategory = parts.GroupBy(p => p.Category);
foreach (var group in byCategory)
{
    Console.WriteLine($"{group.Key}: {group.Count()} parts");
    foreach (var part in group)
        Console.WriteLine($"  {part.Key} ({part.Width}×{part.Height})");
}
```

## Expected directory layout

```
assets/fakemon-parts/
  body/
    dragon_base.png
    serpent_base.png
  head/
    electric_head.png
    fire_head.png
  tail/
    spiked_tail.png
  accessory/
    crown.png
```
