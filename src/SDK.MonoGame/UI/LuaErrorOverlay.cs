namespace SDK.MonoGame.UI;

public sealed class LuaErrorOverlay
{
    public bool HasError { get; private set; }
    public string? LastError { get; private set; }

    public void SetError(string message)
    {
        LastError = message;
        HasError  = true;
        Console.Error.WriteLine($"[LUA HOT RELOAD ERROR] {message}");
    }

    public void ClearError()
    {
        HasError  = false;
        LastError = null;
    }

    // Draw stub — SpriteFont/MGCB déféré Plan 07-04
    public void Draw() { }
}
