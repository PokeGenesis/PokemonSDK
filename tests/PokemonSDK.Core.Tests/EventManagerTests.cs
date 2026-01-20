using Xunit;
using PokemonSDK.Core.Events;
using PokemonSDK.Core.Models;

namespace PokemonSDK.Core.Tests;

public class EventManagerTests
{
    [Fact]
    public void Publish_CallsSubscribedHandler()
    {
        // Arrange
        var eventManager = new EventManager();
        var eventFired = false;
        Pokemon? caughtPokemon = null;
        
        eventManager.Subscribe<PokemonCaughtEvent>(e =>
        {
            eventFired = true;
            caughtPokemon = e.Pokemon;
        });
        
        var pokemon = new Pokemon { Nickname = "Pikachu" };
        var gameEvent = new PokemonCaughtEvent
        {
            Pokemon = pokemon,
            TrainerId = 1
        };
        
        // Act
        eventManager.Publish(gameEvent);
        
        // Assert
        Assert.True(eventFired);
        Assert.NotNull(caughtPokemon);
        Assert.Equal("Pikachu", caughtPokemon.Nickname);
    }
    
    [Fact]
    public void Unsubscribe_RemovesHandler()
    {
        // Arrange
        var eventManager = new EventManager();
        var eventFiredCount = 0;
        
        void handler(PokemonCaughtEvent e) => eventFiredCount++;
        
        eventManager.Subscribe<PokemonCaughtEvent>(handler);
        eventManager.Unsubscribe<PokemonCaughtEvent>(handler);
        
        var gameEvent = new PokemonCaughtEvent
        {
            Pokemon = new Pokemon(),
            TrainerId = 1
        };
        
        // Act
        eventManager.Publish(gameEvent);
        
        // Assert
        Assert.Equal(0, eventFiredCount);
    }
    
    [Fact]
    public void Publish_CallsMultipleHandlers()
    {
        // Arrange
        var eventManager = new EventManager();
        var handler1Called = false;
        var handler2Called = false;
        
        eventManager.Subscribe<PokemonCaughtEvent>(e => handler1Called = true);
        eventManager.Subscribe<PokemonCaughtEvent>(e => handler2Called = true);
        
        var gameEvent = new PokemonCaughtEvent
        {
            Pokemon = new Pokemon(),
            TrainerId = 1
        };
        
        // Act
        eventManager.Publish(gameEvent);
        
        // Assert
        Assert.True(handler1Called);
        Assert.True(handler2Called);
    }
}
