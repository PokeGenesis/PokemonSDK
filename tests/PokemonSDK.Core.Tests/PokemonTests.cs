using Xunit;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Tests;

public class PokemonTests
{
    [Fact]
    public void Pokemon_CalculateStat_ReturnsCorrectHP()
    {
        // Arrange
        var pokemon = new Pokemon
        {
            Level = 50,
            HP_IV = 31,
            HP_EV = 252,
            Nature = Nature.Hardy
        };
        
        var species = new PokemonSpecies
        {
            BaseHP = 100
        };
        
        // Act
        var hp = pokemon.CalculateStat(Stat.HP, species);
        
        // Assert
        Assert.True(hp > 0);
        Assert.Equal(207, hp); // Based on formula
    }
    
    [Fact]
    public void Pokemon_CalculateStat_AppliesNatureCorrectly()
    {
        // Arrange
        var pokemon = new Pokemon
        {
            Level = 50,
            Attack_IV = 31,
            Attack_EV = 0,
            Nature = Nature.Adamant // +Attack -SpAttack
        };
        
        var species = new PokemonSpecies
        {
            BaseAttack = 100
        };
        
        // Act
        var attack = pokemon.CalculateStat(Stat.Attack, species);
        
        // Assert
        Assert.True(attack > 115); // Should be boosted by nature
    }
    
    [Fact]
    public void Pokemon_Constructor_GeneratesUniqueId()
    {
        // Arrange & Act
        var pokemon1 = new Pokemon();
        var pokemon2 = new Pokemon();
        
        // Assert
        Assert.NotEqual(pokemon1.Id, pokemon2.Id);
    }
    
    [Fact]
    public void Pokemon_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var pokemon = new Pokemon();
        
        // Assert
        Assert.Equal(StatusCondition.None, pokemon.Status);
        Assert.Equal(70, pokemon.Friendship);
        Assert.Empty(pokemon.Moves);
    }
}
