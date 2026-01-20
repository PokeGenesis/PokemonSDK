using System.Text.Json;
using PokemonSDK.Core.Models;

namespace PokemonSDK.Core.Data;

/// <summary>
/// Manager for saving and loading game state
/// </summary>
public class SaveManager
{
    /// <summary>
    /// Save trainer data to a file
    /// </summary>
    public void SaveTrainer(Trainer trainer, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        var json = JsonSerializer.Serialize(trainer, options);
        File.WriteAllText(filePath, json);
    }
    
    /// <summary>
    /// Load trainer data from a file
    /// </summary>
    public Trainer? LoadTrainer(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }
        
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Trainer>(json);
    }
    
    /// <summary>
    /// Check if a save file exists
    /// </summary>
    public bool SaveExists(string filePath)
    {
        return File.Exists(filePath);
    }
    
    /// <summary>
    /// Delete a save file
    /// </summary>
    public void DeleteSave(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
