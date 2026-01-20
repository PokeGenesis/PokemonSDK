using Xunit;
using PokemonSDK.Core;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core.Tests;

public class PokemonGameTests
{
    [Fact]
    public void NewGame_CreatesPlayer()
    {
        // Arrange
        var game = new PokemonGame();
        
        // Act
        game.NewGame("Ash", Gender.Male);
        
        // Assert
        Assert.NotNull(game.Player);
        Assert.Equal("Ash", game.Player.Name);
        Assert.Equal(Gender.Male, game.Player.Gender);
        Assert.Equal(3000, game.Player.Money);
    }
    
    [Fact]
    public void AddPokemon_AddsToParty_WhenPartyNotFull()
    {
        // Arrange
        var game = new PokemonGame();
        game.NewGame("Ash", Gender.Male);
        var pokemon = new Pokemon { Nickname = "Pikachu" };
        
        // Act
        game.AddPokemon(pokemon);
        
        // Assert
        Assert.Single(game.Player!.Party);
        Assert.Equal("Pikachu", game.Player.Party[0].Nickname);
    }
    
    [Fact]
    public void AddPokemon_AddsToPC_WhenPartyFull()
    {
        // Arrange
        var game = new PokemonGame();
        game.NewGame("Ash", Gender.Male);
        
        // Fill party
        for (int i = 0; i < 6; i++)
        {
            game.AddPokemon(new Pokemon { Nickname = $"Pokemon{i}" });
        }
        
        // Act
        var newPokemon = new Pokemon { Nickname = "Extra" };
        game.AddPokemon(newPokemon);
        
        // Assert
        Assert.Equal(6, game.Player!.Party.Count);
        Assert.Single(game.Player.PC);
        Assert.Equal("Extra", game.Player.PC[0].Nickname);
    }
    
    [Fact]
    public void StartWildBattle_CreatesBattle()
    {
        // Arrange
        var game = new PokemonGame();
        game.NewGame("Ash", Gender.Male);
        game.AddPokemon(new Pokemon { Nickname = "Pikachu", CurrentHP = 50 });
        var wildPokemon = new Pokemon { Nickname = "Rattata", CurrentHP = 30 };
        
        // Act
        var battle = game.StartWildBattle(wildPokemon);
        
        // Assert
        Assert.NotNull(battle);
        Assert.Equal(Battle.BattleType.Wild, battle.BattleType);
        Assert.Equal(2, battle.Participants.Count);
    }
}
