namespace SDK.Core.Services;

using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using System.Text.Json;

public class SaveSystem : ISaveSystem
{
    public void Save(GameState state, string path)
    {
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public GameState? Load(string path)
    {
        if (!File.Exists(path)) return null;
        return JsonSerializer.Deserialize<GameState>(File.ReadAllText(path));
    }
}
