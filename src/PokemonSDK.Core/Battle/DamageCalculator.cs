using PokemonSDK.Core.Models;
using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Battle;

/// <summary>
/// Calculates damage for moves in battle
/// </summary>
public class DamageCalculator
{
    private static readonly Dictionary<(PokemonType, PokemonType), double> TypeEffectiveness = InitializeTypeChart();
    
    /// <summary>
    /// Calculate damage dealt by a move
    /// </summary>
    public static int CalculateDamage(Pokemon attacker, Pokemon defender, Move move, PokemonSpecies attackerSpecies, PokemonSpecies defenderSpecies, Battle battle)
    {
        if (move.Category == MoveCategory.Status)
        {
            return 0; // Status moves don't deal damage
        }
        
        // Base damage calculation
        var level = attacker.Level;
        var power = move.Power;
        var attack = move.Category == MoveCategory.Physical 
            ? attacker.CalculateStat(Stat.Attack, attackerSpecies)
            : attacker.CalculateStat(Stat.SpecialAttack, attackerSpecies);
        var defense = move.Category == MoveCategory.Physical
            ? defender.CalculateStat(Stat.Defense, defenderSpecies)
            : defender.CalculateStat(Stat.SpecialDefense, defenderSpecies);
        
        // Basic damage formula
        var baseDamage = ((2 * level / 5 + 2) * power * attack / defense) / 50 + 2;
        
        // Apply modifiers
        var modifier = 1.0;
        
        // STAB (Same Type Attack Bonus)
        if (move.Type == attackerSpecies.PrimaryType || move.Type == attackerSpecies.SecondaryType)
        {
            modifier *= 1.5;
        }
        
        // Type effectiveness
        modifier *= GetTypeEffectiveness(move.Type, defenderSpecies.PrimaryType);
        if (defenderSpecies.SecondaryType.HasValue)
        {
            modifier *= GetTypeEffectiveness(move.Type, defenderSpecies.SecondaryType.Value);
        }
        
        // Random factor (0.85 to 1.0)
        var random = new Random();
        modifier *= 0.85 + random.NextDouble() * 0.15;
        
        // Weather modifiers
        if (battle.Weather == Weather.Rain)
        {
            if (move.Type == PokemonType.Water) modifier *= 1.5;
            if (move.Type == PokemonType.Fire) modifier *= 0.5;
        }
        else if (battle.Weather == Weather.HarshSunlight)
        {
            if (move.Type == PokemonType.Fire) modifier *= 1.5;
            if (move.Type == PokemonType.Water) modifier *= 0.5;
        }
        
        return Math.Max(1, (int)(baseDamage * modifier));
    }
    
    /// <summary>
    /// Get type effectiveness multiplier
    /// </summary>
    public static double GetTypeEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (TypeEffectiveness.TryGetValue((attackType, defenseType), out var effectiveness))
        {
            return effectiveness;
        }
        return 1.0; // Normal effectiveness
    }
    
    private static Dictionary<(PokemonType, PokemonType), double> InitializeTypeChart()
    {
        var chart = new Dictionary<(PokemonType, PokemonType), double>();
        
        // Super effective (2x)
        chart[(PokemonType.Fire, PokemonType.Grass)] = 2.0;
        chart[(PokemonType.Fire, PokemonType.Ice)] = 2.0;
        chart[(PokemonType.Fire, PokemonType.Bug)] = 2.0;
        chart[(PokemonType.Fire, PokemonType.Steel)] = 2.0;
        
        chart[(PokemonType.Water, PokemonType.Fire)] = 2.0;
        chart[(PokemonType.Water, PokemonType.Ground)] = 2.0;
        chart[(PokemonType.Water, PokemonType.Rock)] = 2.0;
        
        chart[(PokemonType.Grass, PokemonType.Water)] = 2.0;
        chart[(PokemonType.Grass, PokemonType.Ground)] = 2.0;
        chart[(PokemonType.Grass, PokemonType.Rock)] = 2.0;
        
        chart[(PokemonType.Electric, PokemonType.Water)] = 2.0;
        chart[(PokemonType.Electric, PokemonType.Flying)] = 2.0;
        
        chart[(PokemonType.Fighting, PokemonType.Normal)] = 2.0;
        chart[(PokemonType.Fighting, PokemonType.Ice)] = 2.0;
        chart[(PokemonType.Fighting, PokemonType.Rock)] = 2.0;
        chart[(PokemonType.Fighting, PokemonType.Dark)] = 2.0;
        chart[(PokemonType.Fighting, PokemonType.Steel)] = 2.0;
        
        chart[(PokemonType.Flying, PokemonType.Grass)] = 2.0;
        chart[(PokemonType.Flying, PokemonType.Fighting)] = 2.0;
        chart[(PokemonType.Flying, PokemonType.Bug)] = 2.0;
        
        chart[(PokemonType.Psychic, PokemonType.Fighting)] = 2.0;
        chart[(PokemonType.Psychic, PokemonType.Poison)] = 2.0;
        
        chart[(PokemonType.Bug, PokemonType.Grass)] = 2.0;
        chart[(PokemonType.Bug, PokemonType.Psychic)] = 2.0;
        chart[(PokemonType.Bug, PokemonType.Dark)] = 2.0;
        
        chart[(PokemonType.Rock, PokemonType.Fire)] = 2.0;
        chart[(PokemonType.Rock, PokemonType.Ice)] = 2.0;
        chart[(PokemonType.Rock, PokemonType.Flying)] = 2.0;
        chart[(PokemonType.Rock, PokemonType.Bug)] = 2.0;
        
        chart[(PokemonType.Ghost, PokemonType.Psychic)] = 2.0;
        chart[(PokemonType.Ghost, PokemonType.Ghost)] = 2.0;
        
        chart[(PokemonType.Dragon, PokemonType.Dragon)] = 2.0;
        
        chart[(PokemonType.Dark, PokemonType.Psychic)] = 2.0;
        chart[(PokemonType.Dark, PokemonType.Ghost)] = 2.0;
        
        chart[(PokemonType.Fairy, PokemonType.Fighting)] = 2.0;
        chart[(PokemonType.Fairy, PokemonType.Dragon)] = 2.0;
        chart[(PokemonType.Fairy, PokemonType.Dark)] = 2.0;
        
        // Not very effective (0.5x)
        chart[(PokemonType.Fire, PokemonType.Fire)] = 0.5;
        chart[(PokemonType.Fire, PokemonType.Water)] = 0.5;
        chart[(PokemonType.Fire, PokemonType.Rock)] = 0.5;
        chart[(PokemonType.Fire, PokemonType.Dragon)] = 0.5;
        
        chart[(PokemonType.Water, PokemonType.Water)] = 0.5;
        chart[(PokemonType.Water, PokemonType.Grass)] = 0.5;
        chart[(PokemonType.Water, PokemonType.Dragon)] = 0.5;
        
        chart[(PokemonType.Grass, PokemonType.Fire)] = 0.5;
        chart[(PokemonType.Grass, PokemonType.Grass)] = 0.5;
        chart[(PokemonType.Grass, PokemonType.Poison)] = 0.5;
        chart[(PokemonType.Grass, PokemonType.Flying)] = 0.5;
        chart[(PokemonType.Grass, PokemonType.Bug)] = 0.5;
        chart[(PokemonType.Grass, PokemonType.Dragon)] = 0.5;
        chart[(PokemonType.Grass, PokemonType.Steel)] = 0.5;
        
        chart[(PokemonType.Electric, PokemonType.Grass)] = 0.5;
        chart[(PokemonType.Electric, PokemonType.Electric)] = 0.5;
        chart[(PokemonType.Electric, PokemonType.Dragon)] = 0.5;
        
        // No effect (0x)
        chart[(PokemonType.Normal, PokemonType.Ghost)] = 0.0;
        chart[(PokemonType.Fighting, PokemonType.Ghost)] = 0.0;
        chart[(PokemonType.Electric, PokemonType.Ground)] = 0.0;
        chart[(PokemonType.Ground, PokemonType.Flying)] = 0.0;
        chart[(PokemonType.Psychic, PokemonType.Dark)] = 0.0;
        chart[(PokemonType.Ghost, PokemonType.Normal)] = 0.0;
        chart[(PokemonType.Dragon, PokemonType.Fairy)] = 0.0;
        
        return chart;
    }
}
