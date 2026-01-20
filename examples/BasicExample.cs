using PokemonSDK.Core;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.Enums;
using PokemonSDK.Core.Battle;

namespace PokemonSDK.Examples;

/// <summary>
/// Example demonstrating basic Pokemon SDK usage
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== PokemonSDK C# Example ===\n");
        
        // Create a new game
        var game = new PokemonGame();
        game.NewGame("Ash", Gender.Male);
        
        Console.WriteLine($"Player: {game.Player!.Name}");
        Console.WriteLine($"Starting money: ${game.Player.Money}\n");
        
        // Create Pokemon species
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
            BaseSpeed = 90,
            CatchRate = 190
        };
        
        var charmanderSpecies = new PokemonSpecies
        {
            Id = 4,
            Name = "Charmander",
            PrimaryType = PokemonType.Fire,
            BaseHP = 39,
            BaseAttack = 52,
            BaseDefense = 43,
            BaseSpecialAttack = 60,
            BaseSpecialDefense = 50,
            BaseSpeed = 65
        };
        
        // Create Pokemon instances
        var pikachu = CreatePokemon(25, "Pikachu", 10, Nature.Jolly);
        var charmander = CreatePokemon(4, "Charmander", 8, Nature.Adamant);
        
        // Calculate and display stats
        Console.WriteLine("=== Pokemon Stats ===");
        DisplayPokemonStats(pikachu, pikachuSpecies);
        Console.WriteLine();
        DisplayPokemonStats(charmander, charmanderSpecies);
        
        // Add Pokemon to party
        game.AddPokemon(pikachu);
        game.AddPokemon(charmander);
        
        Console.WriteLine($"\n=== Party ===");
        Console.WriteLine($"Party size: {game.Player.GetPartySize()}");
        
        // Create a move
        var thunderbolt = new Move
        {
            Id = 85,
            Name = "Thunderbolt",
            Type = PokemonType.Electric,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            PP = 15
        };
        
        var ember = new Move
        {
            Id = 52,
            Name = "Ember",
            Type = PokemonType.Fire,
            Category = MoveCategory.Special,
            Power = 40,
            Accuracy = 100,
            PP = 25
        };
        
        // Add moves to Pokemon
        pikachu.Moves.Add(new PokemonMove { MoveId = 85, CurrentPP = 15, MaxPP = 15 });
        charmander.Moves.Add(new PokemonMove { MoveId = 52, CurrentPP = 25, MaxPP = 25 });
        
        // Demonstrate type effectiveness
        Console.WriteLine("\n=== Type Effectiveness ===");
        var effectiveness1 = DamageCalculator.GetTypeEffectiveness(PokemonType.Water, PokemonType.Fire);
        Console.WriteLine($"Water vs Fire: {effectiveness1}x");
        
        var effectiveness2 = DamageCalculator.GetTypeEffectiveness(PokemonType.Electric, PokemonType.Water);
        Console.WriteLine($"Electric vs Water: {effectiveness2}x");
        
        var effectiveness3 = DamageCalculator.GetTypeEffectiveness(PokemonType.Normal, PokemonType.Ghost);
        Console.WriteLine($"Normal vs Ghost: {effectiveness3}x");
        
        // Demonstrate damage calculation
        Console.WriteLine("\n=== Battle Simulation ===");
        var battle = new Battle.Battle();
        
        // Calculate damage
        var damage1 = DamageCalculator.CalculateDamage(
            pikachu, charmander, thunderbolt, 
            pikachuSpecies, charmanderSpecies, battle
        );
        Console.WriteLine($"{pikachu.Nickname} uses Thunderbolt on {charmander.Nickname}!");
        Console.WriteLine($"Damage: {damage1} HP");
        
        var damage2 = DamageCalculator.CalculateDamage(
            charmander, pikachu, ember,
            charmanderSpecies, pikachuSpecies, battle
        );
        Console.WriteLine($"\n{charmander.Nickname} uses Ember on {pikachu.Nickname}!");
        Console.WriteLine($"Damage: {damage2} HP");
        
        // Inventory demonstration
        Console.WriteLine("\n=== Inventory ===");
        game.Player.Bag.AddItem(1, 5); // Potion
        game.Player.Bag.AddItem(4, 3); // Pokeball
        Console.WriteLine($"Potions: {game.Player.Bag.GetItemCount(1)}");
        Console.WriteLine($"Pokeballs: {game.Player.Bag.GetItemCount(4)}");
        
        game.Player.Bag.RemoveItem(1, 2);
        Console.WriteLine($"After using 2 potions: {game.Player.Bag.GetItemCount(1)}");
        
        Console.WriteLine("\n=== Example Complete ===");
    }
    
    static Pokemon CreatePokemon(int speciesId, string nickname, int level, Nature nature)
    {
        var random = new Random();
        return new Pokemon
        {
            SpeciesId = speciesId,
            Nickname = nickname,
            Level = level,
            Nature = nature,
            HP_IV = random.Next(0, 32),
            Attack_IV = random.Next(0, 32),
            Defense_IV = random.Next(0, 32),
            SpecialAttack_IV = random.Next(0, 32),
            SpecialDefense_IV = random.Next(0, 32),
            Speed_IV = random.Next(0, 32),
            CurrentHP = 100,
            Gender = random.Next(0, 2) == 0 ? Gender.Male : Gender.Female
        };
    }
    
    static void DisplayPokemonStats(Pokemon pokemon, PokemonSpecies species)
    {
        Console.WriteLine($"{pokemon.Nickname} (Lv.{pokemon.Level}) - {pokemon.Nature}");
        Console.WriteLine($"  HP: {pokemon.CalculateStat(Stat.HP, species)}");
        Console.WriteLine($"  Attack: {pokemon.CalculateStat(Stat.Attack, species)}");
        Console.WriteLine($"  Defense: {pokemon.CalculateStat(Stat.Defense, species)}");
        Console.WriteLine($"  Sp.Atk: {pokemon.CalculateStat(Stat.SpecialAttack, species)}");
        Console.WriteLine($"  Sp.Def: {pokemon.CalculateStat(Stat.SpecialDefense, species)}");
        Console.WriteLine($"  Speed: {pokemon.CalculateStat(Stat.Speed, species)}");
    }
}
