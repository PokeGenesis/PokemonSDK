# PokeAPI Integration and Multi-Language Support

## Features Overview

This implementation adds official Pokemon data from PokeAPI with multi-language support and database storage.

### Supported Languages
- English
- French (Français)
- Spanish (Español)

### Database Storage
All Pokemon data is stored in a SQLite database using Entity Framework Core:
- Pokemon Species
- Moves
- Abilities
- Items
- Trainers & Pokemon instances

### Data Filtering
Advanced filtering options for all data types:
- Enable/disable individual data entries
- Filter by type
- Filter by generation
- Filter by level range (for applicable data)

## Quick Start

### 1. Initialize Database

```csharp
using Microsoft.EntityFrameworkCore;
using PokemonSDK.Core.Database;
using PokemonSDK.Core.Data;
using PokemonSDK.Core.PokeApi;

// Configure database
var options = new DbContextOptionsBuilder<PokemonDbContext>()
    .UseSqlite("Data Source=pokemon.db")
    .Options;

var dbContext = new PokemonDbContext(options);
await dbContext.Database.EnsureCreatedAsync();
```

### 2. Import Data from PokeAPI

```csharp
var pokeApiClient = new PokeApiClient();
var dataManager = new EnhancedDataManager(dbContext, pokeApiClient);

// Import Pikachu (#25)
var pikachu = await dataManager.ImportSpeciesFromPokeApiAsync(25);

// Import Thunderbolt move (#85)
var thunderbolt = await dataManager.ImportMoveFromPokeApiAsync(85);

// Import Static ability (#9)
var staticAbility = await dataManager.ImportAbilityFromPokeApiAsync(9);
```

### 3. Use Multi-Language Support

```csharp
using PokemonSDK.Core.Localization;

// Get localized names
string englishName = pikachu.Name; // "pikachu"
string frenchName = pikachu.GetLocalizedName(Language.French); // "Pikachu"
string spanishName = pikachu.GetLocalizedName(Language.Spanish); // "Pikachu"

// Get localized descriptions
string description = pikachu.GetLocalizedDescription(Language.French);
```

### 4. Filter Data

```csharp
// Create filter options
var filterOptions = new DataFilterOptions
{
    Category = "Species",
    EnabledOnly = true,
    TypeFilter = "Electric",
    Generation = "1"
};

// Get filtered Pokemon
var electricPokemon = await dataManager
    .GetFilteredSpecies(filterOptions)
    .ToListAsync();

// Filter by multiple criteria
var gen1FireTypes = await dataManager
    .GetFilteredSpecies(new DataFilterOptions 
    { 
        EnabledOnly = true,
        TypeFilter = "Fire",
        Generation = "1"
    })
    .ToListAsync();
```

### 5. Enable/Disable Data

```csharp
// Disable a Pokemon species
pikachu.IsEnabled = false;
await dbContext.SaveChangesAsync();

// Query only enabled species
var enabledSpecies = await dataManager
    .GetFilteredSpecies(new DataFilterOptions { EnabledOnly = true })
    .ToListAsync();
```

## PokeAPI Client Methods

```csharp
var client = new PokeApiClient();

// Get Pokemon species
var species = await client.GetPokemonSpeciesAsync("pikachu");

// Get Pokemon details
var pokemon = await client.GetPokemonAsync("25");

// Get move
var move = await client.GetMoveAsync("thunderbolt");

// Get ability
var ability = await client.GetAbilityAsync("static");

// List Pokemon (with pagination)
var list = await client.GetPokemonListAsync(limit: 151, offset: 0);
```

## Database Schema

### PokemonSpecies Table
- Multi-language names (English, French, Spanish)
- Multi-language descriptions
- Base stats (HP, Attack, Defense, Sp.Atk, Sp.Def, Speed)
- Types (Primary, Secondary)
- Generation
- IsEnabled flag

### Moves Table
- Multi-language names and descriptions
- Type, Category, Power, PP, Accuracy
- IsEnabled flag

### Abilities Table
- Multi-language names and descriptions
- IsEnabled flag

### Items Table
- Multi-language names and descriptions
- Type, Price
- IsEnabled flag

## Advanced Features

### Batch Import

```csharp
// Import first generation Pokemon (1-151)
for (int i = 1; i <= 151; i++)
{
    try
    {
        await dataManager.ImportSpeciesFromPokeApiAsync(i);
        Console.WriteLine($"Imported Pokemon #{i}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to import Pokemon #{i}: {ex.Message}");
    }
}
```

### Query with Language

```csharp
// Get all Pokemon with French names
var allPokemon = await dbContext.Species.ToListAsync();
foreach (var pokemon in allPokemon)
{
    Console.WriteLine($"{pokemon.Id}: {pokemon.GetLocalizedName(Language.French)}");
}
```

### Complex Filtering

```csharp
// Get all enabled dual-type Pokemon from Generation 1
var dualTypes = await dataManager
    .GetFilteredSpecies(new DataFilterOptions 
    { 
        EnabledOnly = true,
        Generation = "1"
    })
    .Where(s => s.SecondaryType != null)
    .ToListAsync();
```

## Configuration

Add to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PokemonDb": "Data Source=pokemon.db"
  },
  "PokeApi": {
    "BaseUrl": "https://pokeapi.co/api/v2/",
    "DefaultLanguage": "en"
  }
}
```

## Best Practices

1. **Rate Limiting**: PokeAPI has rate limits. Implement delays between requests when importing large amounts of data.

2. **Caching**: Once data is imported to the database, query from the database instead of making repeated API calls.

3. **Error Handling**: Always wrap API calls in try-catch blocks as network requests can fail.

4. **Localization**: Check for empty strings when getting localized content and fall back to English.

5. **Filtering**: Use database-level filtering (`GetFilteredSpecies`) instead of loading all data and filtering in memory.

## Troubleshooting

### Database Connection Issues
```csharp
// Verify database exists
if (!await dbContext.Database.CanConnectAsync())
{
    await dbContext.Database.EnsureCreatedAsync();
}
```

### API Rate Limiting
```csharp
// Add delay between requests
for (int i = 1; i <= 151; i++)
{
    await dataManager.ImportSpeciesFromPokeApiAsync(i);
    await Task.Delay(100); // 100ms delay
}
```

### Missing Translations
```csharp
// Check if translation exists
string name = pikachu.GetLocalizedName(Language.French);
if (string.IsNullOrEmpty(name))
{
    name = pikachu.Name; // Fallback to English
}
```
