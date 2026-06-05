namespace SDK.Core.ValueObjects;

using System.Text.Json;

public record GameState
{
    public string PlayerName    { get; init; } = string.Empty;
    public int    PlaytimeSeconds { get; init; } = 0;
    // D-12 : flags/badges stockés comme JsonElement pour round-trip JSON sans perte de type
    public Dictionary<string, JsonElement> Flags { get; init; } = new();

    public GameState WithFlag(string key, bool value)
    {
        var updated = new Dictionary<string, JsonElement>(Flags)
        {
            [key] = JsonSerializer.SerializeToElement(value)
        };
        return this with { Flags = updated };
    }

    public T? GetFlag<T>(string key)
    {
        if (!Flags.TryGetValue(key, out var element)) return default;
        return JsonSerializer.Deserialize<T>(element.GetRawText());
    }
}
