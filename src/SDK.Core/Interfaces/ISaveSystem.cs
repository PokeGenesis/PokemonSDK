namespace SDK.Core.Interfaces;

using SDK.Core.ValueObjects;

public interface ISaveSystem
{
    void Save(GameState state, string path);
    GameState? Load(string path);
}
