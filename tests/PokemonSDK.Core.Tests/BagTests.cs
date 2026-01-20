using Xunit;
using PokemonSDK.Core.Inventory;

namespace PokemonSDK.Core.Tests;

public class BagTests
{
    [Fact]
    public void AddItem_AddsNewItem()
    {
        // Arrange
        var bag = new Bag();
        
        // Act
        bag.AddItem(1, 5);
        
        // Assert
        Assert.Equal(5, bag.GetItemCount(1));
    }
    
    [Fact]
    public void AddItem_IncreasesExistingItem()
    {
        // Arrange
        var bag = new Bag();
        bag.AddItem(1, 5);
        
        // Act
        bag.AddItem(1, 3);
        
        // Assert
        Assert.Equal(8, bag.GetItemCount(1));
    }
    
    [Fact]
    public void RemoveItem_ReducesQuantity()
    {
        // Arrange
        var bag = new Bag();
        bag.AddItem(1, 5);
        
        // Act
        var result = bag.RemoveItem(1, 2);
        
        // Assert
        Assert.True(result);
        Assert.Equal(3, bag.GetItemCount(1));
    }
    
    [Fact]
    public void RemoveItem_RemovesItemWhenQuantityZero()
    {
        // Arrange
        var bag = new Bag();
        bag.AddItem(1, 5);
        
        // Act
        var result = bag.RemoveItem(1, 5);
        
        // Assert
        Assert.True(result);
        Assert.Equal(0, bag.GetItemCount(1));
        Assert.False(bag.HasItem(1));
    }
    
    [Fact]
    public void RemoveItem_FailsWhenNotEnough()
    {
        // Arrange
        var bag = new Bag();
        bag.AddItem(1, 3);
        
        // Act
        var result = bag.RemoveItem(1, 5);
        
        // Assert
        Assert.False(result);
        Assert.Equal(3, bag.GetItemCount(1));
    }
    
    [Fact]
    public void HasItem_ReturnsTrueWhenItemExists()
    {
        // Arrange
        var bag = new Bag();
        bag.AddItem(1, 1);
        
        // Act & Assert
        Assert.True(bag.HasItem(1));
    }
    
    [Fact]
    public void HasItem_ReturnsFalseWhenItemDoesNotExist()
    {
        // Arrange
        var bag = new Bag();
        
        // Act & Assert
        Assert.False(bag.HasItem(1));
    }
}
