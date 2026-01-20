# PokemonSDK

A modern C# implementation of the PokemonSDK - a comprehensive game development framework for creating Pokémon-style games with improved architecture, performance, **official PokeAPI data**, **multi-language support**, and **database storage**.

## Features

### Core Systems
- **Pokemon Management**: Complete Pokemon species, stats, IVs, EVs, natures, abilities
- **Battle System**: Turn-based battle mechanics with type effectiveness, damage calculation
- **Move System**: Comprehensive move system with physical, special, and status moves
- **Trainer System**: Player and NPC trainer management with parties and PC storage
- **Inventory System**: Item management with bag system
- **Evolution System**: Pokemon evolution with multiple methods and conditions
- **Event System**: Event-driven architecture for game events
- **Data Management**: JSON-based data loading and save system

### NEW: PokeAPI Integration & Multi-Language Support 🌍
- **Official Data**: Import Pokemon data directly from [PokeAPI](https://pokeapi.co/)
- **Multi-Language**: Full support for English, French (Français), and Spanish (Español)
- **Database Storage**: SQLite database with Entity Framework Core
- **Data Filtering**: Advanced filtering with enable/disable options for all data types
- **Generation Support**: Filter Pokemon by generation (Gen 1-9)
- **Type Filtering**: Filter by Pokemon types

### Improvements over Ruby SDK
- **Type Safety**: Strong typing with C# ensures fewer runtime errors
- **Performance**: Better performance with compiled C# vs interpreted Ruby (10-100x faster)
- **Modern Architecture**: Clean, modular design with SOLID principles
- **Comprehensive Testing**: Unit tests for all core functionality
- **Cross-Platform**: Runs on Windows, Linux, and macOS via .NET
- **Better IDE Support**: Full IntelliSense and debugging support
- **Official Data**: Direct integration with PokeAPI for authentic Pokemon data
- **Multi-Language**: Native support for multiple languages

## Getting Started

### Prerequisites
- .NET 10.0 SDK or later

### Installation
```bash
# Clone the repository
git clone https://github.com/PokeGenesis/PokemonSDK.git
cd PokemonSDK

# Build the project
dotnet build

# Run tests
dotnet test
```

### Quick Example with PokeAPI

```csharp
using Microsoft.EntityFrameworkCore;
using PokemonSDK.Core;
using PokemonSDK.Core.Database;
using PokemonSDK.Core.Data;
using PokemonSDK.Core.PokeApi;
using PokemonSDK.Core.Localization;

// Initialize database
var options = new DbContextOptionsBuilder<PokemonDbContext>()
    .UseSqlite("Data Source=pokemon.db")
    .Options;

var dbContext = new PokemonDbContext(options);
await dbContext.Database.EnsureCreatedAsync();

// Create data manager with PokeAPI integration
var dataManager = new EnhancedDataManager(dbContext, new PokeApiClient());

// Import Pikachu from PokeAPI (with multi-language support)
var pikachu = await dataManager.ImportSpeciesFromPokeApiAsync(25);

Console.WriteLine($"English: {pikachu.Name}");
Console.WriteLine($"French: {pikachu.GetLocalizedName(Language.French)}");
Console.WriteLine($"Spanish: {pikachu.GetLocalizedName(Language.Spanish)}");
Console.WriteLine($"Description (EN): {pikachu.Description}");

// Filter enabled Electric-type Pokemon from Generation 1
var filterOptions = new DataFilterOptions
{
    EnabledOnly = true,
    TypeFilter = "Electric",
    Generation = "1"
};

var electricPokemon = await dataManager
    .GetFilteredSpecies(filterOptions)
    .ToListAsync();

Console.WriteLine($"Found {electricPokemon.Count} Electric Pokemon in Gen 1");
```

### Standard Example

```csharp
using PokemonSDK.Core;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.Enums;

// Create a new game
var game = new PokemonGame();
game.NewGame("Ash", Gender.Male);

// Create a Pokemon species
var pikachuSpecies = new PokemonSpecies
{
    Id = 25,
    Name = "Pikachu",
    NameFrench = "Pikachu",
    NameSpanish = "Pikachu",
    PrimaryType = PokemonType.Electric,
    BaseHP = 35,
    BaseAttack = 55,
    BaseDefense = 40,
    BaseSpecialAttack = 50,
    BaseSpecialDefense = 50,
    BaseSpeed = 90
};

// Create a Pokemon instance
var pikachu = new Pokemon
{
    SpeciesId = 25,
    Nickname = "Pikachu",
    Level = 5,
    HP_IV = 31,
    Attack_IV = 31,
    Nature = Nature.Jolly,
    CurrentHP = 20
};

// Add to player's party
game.AddPokemon(pikachu);

// Calculate stats
var hp = pikachu.CalculateStat(Stat.HP, pikachuSpecies);
var speed = pikachu.CalculateStat(Stat.Speed, pikachuSpecies);

Console.WriteLine($"{pikachu.Nickname} has {hp} HP and {speed} Speed!");
```

## Documentation

- **[POKEAPI_GUIDE.md](POKEAPI_GUIDE.md)** - Complete guide for PokeAPI integration, multi-language support, and database usage
- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Technical implementation details

## Project Structure

```
PokemonSDK/
├── src/
│   └── PokemonSDK.Core/          # Core SDK library
│       ├── Battle/               # Battle system
│       ├── Data/                 # Data management & PokeAPI integration
│       ├── Database/             # EF Core database context
│       ├── Enums/                # Enumerations
│       ├── Events/               # Event system
│       ├── Inventory/            # Item/bag system
│       ├── Localization/         # Multi-language support
│       ├── Models/               # Domain models (with localization)
│       └── PokeApi/              # PokeAPI client
└── tests/
    └── PokemonSDK.Core.Tests/    # Unit tests
```

## Core Components

### Pokemon System
- **PokemonSpecies**: Base stats and characteristics for each species (multi-language)
- **Pokemon**: Individual Pokemon instances with IVs, EVs, levels, moves
- **Nature**: 25 natures affecting stat growth
- **Ability**: Pokemon abilities with effects (multi-language)

### Battle System
- **Battle**: Battle state management
- **BattleAction**: Move, switch, item, and flee actions
- **DamageCalculator**: Complete damage calculation with type effectiveness
- **Weather & Terrain**: Environmental effects on battles

### Data Management
- **EnhancedDataManager**: Import from PokeAPI, store in database, query with filters
- **PokeApiClient**: Async client for fetching official Pokemon data
- **PokemonDbContext**: Entity Framework Core database context
- **DataFilterOptions**: Advanced filtering (type, generation, enabled/disabled)

### Localization
- **LocalizedText**: Multi-language text support
- **Language**: English, French, Spanish
- All models support `GetLocalizedName()` and `GetLocalizedDescription()`

### Event System
- **EventManager**: Pub/sub event system
- **GameEvents**: Pre-defined events (Pokemon caught, evolved, leveled up, etc.)

## Multi-Language Support

All Pokemon data supports three languages:
- **English** (en) - Default
- **French** (fr) - Français
- **Spanish** (es) - Español

```csharp
// Get localized content
string frenchName = pikachu.GetLocalizedName(Language.French);
string spanishDesc = thunderbolt.GetLocalizedDescription(Language.Spanish);
```

## Database Features

- **SQLite** with Entity Framework Core
- **Automatic schema creation**
- **Enable/disable data entries**
- **Advanced filtering and querying**
- **Relationship management**

## Contributing

Contributions are welcome! This project aims to create a better, more maintainable Pokemon game development framework with official data and multi-language support.

## License

See [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by the original Ruby PokemonSDK at https://gitlab.com/pokemonsdk
- Pokemon data from [PokeAPI](https://pokeapi.co/) - The RESTful Pokémon API
- Built with modern C# and .NET for improved performance and developer experience
 
