using Xunit;
using PokemonSDK.Core.Battle;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Tests;

public class DamageCalculatorTests
{
    [Fact]
    public void CalculateDamage_StatusMove_ReturnsZero()
    {
        // Arrange
        var attacker = CreateTestPokemon(50);
        var defender = CreateTestPokemon(50);
        var attackerSpecies = CreateTestSpecies();
        var defenderSpecies = CreateTestSpecies();
        var battle = new Battle.Battle();
        
        var move = new Move
        {
            Category = MoveCategory.Status,
            Power = 0
        };
        
        // Act
        var damage = DamageCalculator.CalculateDamage(attacker, defender, move, attackerSpecies, defenderSpecies, battle);
        
        // Assert
        Assert.Equal(0, damage);
    }
    
    [Fact]
    public void CalculateDamage_PhysicalMove_ReturnsPositiveDamage()
    {
        // Arrange
        var attacker = CreateTestPokemon(50);
        var defender = CreateTestPokemon(50);
        var attackerSpecies = CreateTestSpecies();
        var defenderSpecies = CreateTestSpecies();
        var battle = new Battle.Battle();
        
        var move = new Move
        {
            Category = MoveCategory.Physical,
            Power = 80,
            Type = PokemonType.Normal
        };
        
        // Act
        var damage = DamageCalculator.CalculateDamage(attacker, defender, move, attackerSpecies, defenderSpecies, battle);
        
        // Assert
        Assert.True(damage > 0);
    }
    
    [Fact]
    public void GetTypeEffectiveness_SuperEffective_Returns2()
    {
        // Act
        var effectiveness = DamageCalculator.GetTypeEffectiveness(PokemonType.Fire, PokemonType.Grass);
        
        // Assert
        Assert.Equal(2.0, effectiveness);
    }
    
    [Fact]
    public void GetTypeEffectiveness_NotVeryEffective_Returns0_5()
    {
        // Act
        var effectiveness = DamageCalculator.GetTypeEffectiveness(PokemonType.Fire, PokemonType.Water);
        
        // Assert
        Assert.Equal(0.5, effectiveness);
    }
    
    [Fact]
    public void GetTypeEffectiveness_NoEffect_Returns0()
    {
        // Act
        var effectiveness = DamageCalculator.GetTypeEffectiveness(PokemonType.Normal, PokemonType.Ghost);
        
        // Assert
        Assert.Equal(0.0, effectiveness);
    }
    
    private Pokemon CreateTestPokemon(int level)
    {
        return new Pokemon
        {
            Level = level,
            HP_IV = 31,
            Attack_IV = 31,
            Defense_IV = 31,
            SpecialAttack_IV = 31,
            SpecialDefense_IV = 31,
            Speed_IV = 31,
            Nature = Nature.Hardy
        };
    }
    
    private PokemonSpecies CreateTestSpecies()
    {
        return new PokemonSpecies
        {
            BaseHP = 100,
            BaseAttack = 100,
            BaseDefense = 100,
            BaseSpecialAttack = 100,
            BaseSpecialDefense = 100,
            BaseSpeed = 100,
            PrimaryType = PokemonType.Normal
        };
    }
}
