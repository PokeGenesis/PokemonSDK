---
sidebar_position: 4
---

# FakemonPartsCatalog

Scanne un répertoire d'images de parties Fakemon et fournit lookup et filtrage pour le pipeline d'assemblage.

## Constructeur

```csharp
public FakemonPartsCatalog(string partsDirectory)
```

`partsDirectory` doit contenir des sous-dossiers par catégorie (`body/`, `head/`, `tail/`, `accessory/`).

## Scan

```csharp
public IReadOnlyList<FakemonPart> Scan();
```

Retourne tous les fichiers `.png` découverts dans le répertoire de parties, parsés en records `FakemonPart`.

### FakemonPart

```csharp
public record FakemonPart
{
    public string Key { get; init; }         // nom de fichier sans extension
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

Recherche une partie spécifique par clé et catégorie. Retourne `null` si non trouvée.

```csharp
var tete = catalog.GetPart("electric_head", "head");
if (tete is null)
    throw new InvalidOperationException("Partie 'electric_head' introuvable");
```

## Filter

```csharp
public IReadOnlyList<FakemonPart> Filter(string category);
```

Retourne toutes les parties d'une catégorie donnée.

```csharp
var toutesLesTetes = catalog.Filter("head");
Console.WriteLine($"{toutesLesTetes.Count} parties de tête disponibles");
```

## Exemple

```csharp
var catalog = new FakemonPartsCatalog("assets/fakemon-parts/");
var parties = catalog.Scan();

// Grouper par catégorie
var parCategorie = parties.GroupBy(p => p.Category);
foreach (var groupe in parCategorie)
{
    Console.WriteLine($"{groupe.Key} : {groupe.Count()} partie(s)");
    foreach (var partie in groupe)
        Console.WriteLine($"  {partie.Key} ({partie.Width}×{partie.Height})");
}
```

## Structure de répertoire attendue

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
    couronne.png
```
