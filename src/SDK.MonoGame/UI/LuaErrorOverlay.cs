namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    public void Draw(SpriteBatch sb, SpriteFont? font)
    {
        if (!HasError || font is null) return;
        sb.Begin();
        sb.DrawString(font, $"[LUA ERROR] {LastError}", new Vector2(10, 10), Color.Red);
        sb.End();
    }
}
