namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DialogueBox
{
    public bool IsOpen { get; private set; }
    public string CurrentText { get; private set; } = string.Empty;

    public void Open(string text)
    {
        CurrentText = text;
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public void Draw(SpriteBatch sb, SpriteFont? font)
    {
        if (!IsOpen || font is null) return;
        sb.Begin();
        sb.DrawString(font, CurrentText, new Vector2(20, 1048), Color.White);
        sb.End();
    }
}
