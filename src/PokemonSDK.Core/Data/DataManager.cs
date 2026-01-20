using System.Text.Json;
using PokemonSDK.Core.Models;

namespace PokemonSDK.Core.Data;

/// <summary>
/// Manager for loading and accessing game data
/// </summary>
public class DataManager
{
    private readonly Dictionary<int, PokemonSpecies> _species = new();
    private readonly Dictionary<int, Move> _moves = new();
    private readonly Dictionary<int, Ability> _abilities = new();
    private readonly Dictionary<int, Inventory.Item> _items = new();
    
    /// <summary>
    /// Load Pokemon species data from a JSON file
    /// </summary>
    public void LoadSpecies(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var species = JsonSerializer.Deserialize<List<PokemonSpecies>>(json);
        
        if (species != null)
        {
            foreach (var s in species)
            {
                _species[s.Id] = s;
            }
        }
    }
    
    /// <summary>
    /// Load moves data from a JSON file
    /// </summary>
    public void LoadMoves(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var moves = JsonSerializer.Deserialize<List<Move>>(json);
        
        if (moves != null)
        {
            foreach (var m in moves)
            {
                _moves[m.Id] = m;
            }
        }
    }
    
    /// <summary>
    /// Load abilities data from a JSON file
    /// </summary>
    public void LoadAbilities(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var abilities = JsonSerializer.Deserialize<List<Ability>>(json);
        
        if (abilities != null)
        {
            foreach (var a in abilities)
            {
                _abilities[a.Id] = a;
            }
        }
    }
    
    /// <summary>
    /// Load items data from a JSON file
    /// </summary>
    public void LoadItems(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var items = JsonSerializer.Deserialize<List<Inventory.Item>>(json);
        
        if (items != null)
        {
            foreach (var i in items)
            {
                _items[i.Id] = i;
            }
        }
    }
    
    /// <summary>
    /// Get a Pokemon species by ID
    /// </summary>
    public PokemonSpecies? GetSpecies(int id)
    {
        return _species.TryGetValue(id, out var species) ? species : null;
    }
    
    /// <summary>
    /// Get a move by ID
    /// </summary>
    public Move? GetMove(int id)
    {
        return _moves.TryGetValue(id, out var move) ? move : null;
    }
    
    /// <summary>
    /// Get an ability by ID
    /// </summary>
    public Ability? GetAbility(int id)
    {
        return _abilities.TryGetValue(id, out var ability) ? ability : null;
    }
    
    /// <summary>
    /// Get an item by ID
    /// </summary>
    public Inventory.Item? GetItem(int id)
    {
        return _items.TryGetValue(id, out var item) ? item : null;
    }
    
    /// <summary>
    /// Get all Pokemon species
    /// </summary>
    public IEnumerable<PokemonSpecies> GetAllSpecies()
    {
        return _species.Values;
    }
    
    /// <summary>
    /// Get all moves
    /// </summary>
    public IEnumerable<Move> GetAllMoves()
    {
        return _moves.Values;
    }
}
