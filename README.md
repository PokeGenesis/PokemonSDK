# PokemonSDK

A modern C# implementation of the PokemonSDK - a comprehensive game development framework for creating Pokémon-style games with improved architecture and performance.

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

### Improvements over Ruby SDK
- **Type Safety**: Strong typing with C# ensures fewer runtime errors
- **Performance**: Better performance with compiled C# vs interpreted Ruby
- **Modern Architecture**: Clean, modular design with SOLID principles
- **Comprehensive Testing**: Unit tests for all core functionality
- **Cross-Platform**: Runs on Windows, Linux, and macOS via .NET
- **Better IDE Support**: Full IntelliSense and debugging support

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

### Quick Example

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

## Project Structure

```
PokemonSDK/
├── src/
│   └── PokemonSDK.Core/          # Core SDK library
│       ├── Battle/               # Battle system
│       ├── Data/                 # Data management
│       ├── Enums/                # Enumerations
│       ├── Events/               # Event system
│       ├── Inventory/            # Item/bag system
│       └── Models/               # Domain models
└── tests/
    └── PokemonSDK.Core.Tests/    # Unit tests
```

## Core Components

### Pokemon System
- **PokemonSpecies**: Base stats and characteristics for each species
- **Pokemon**: Individual Pokemon instances with IVs, EVs, levels, moves
- **Nature**: 25 natures affecting stat growth
- **Ability**: Pokemon abilities with effects

### Battle System
- **Battle**: Battle state management
- **BattleAction**: Move, switch, item, and flee actions
- **DamageCalculator**: Complete damage calculation with type effectiveness
- **Weather & Terrain**: Environmental effects on battles

### Data Management
- **DataManager**: Load Pokemon, moves, abilities, and items from JSON
- **SaveManager**: Save and load game state

### Event System
- **EventManager**: Pub/sub event system
- **GameEvents**: Pre-defined events (Pokemon caught, evolved, leveled up, etc.)

## Contributing

Contributions are welcome! This project aims to create a better, more maintainable Pokemon game development framework.

## License

See [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by the original Ruby PokemonSDK at https://gitlab.com/pokemonsdk
- Built with modern C# and .NET for improved performance and developer experience
 
