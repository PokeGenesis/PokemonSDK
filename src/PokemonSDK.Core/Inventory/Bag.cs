namespace PokemonSDK.Core.Inventory;

/// <summary>
/// Represents the player's bag/inventory
/// </summary>
public class Bag
{
    public Dictionary<int, int> Items { get; set; } = new(); // Item ID -> Quantity
    
    public void AddItem(int itemId, int quantity = 1)
    {
        if (Items.ContainsKey(itemId))
        {
            Items[itemId] += quantity;
        }
        else
        {
            Items[itemId] = quantity;
        }
    }
    
    public bool RemoveItem(int itemId, int quantity = 1)
    {
        if (!Items.ContainsKey(itemId) || Items[itemId] < quantity)
        {
            return false;
        }
        
        Items[itemId] -= quantity;
        if (Items[itemId] <= 0)
        {
            Items.Remove(itemId);
        }
        
        return true;
    }
    
    public int GetItemCount(int itemId)
    {
        return Items.TryGetValue(itemId, out var count) ? count : 0;
    }
    
    public bool HasItem(int itemId)
    {
        return Items.ContainsKey(itemId) && Items[itemId] > 0;
    }
}
