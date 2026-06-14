namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public sealed class BattleEndOverlay
{
    public void Draw(SpriteBatch sb, bool playerWon, SpriteFont? font)
    {
        if (font is null) return;

        // Dialog-style: big result text + prompt, inside bottom panel (y 178–270)
        var msg = playerWon ? "YOU WIN!" : "YOU LOSE...";
        var msgPos = new Vector2(148, 193);
        sb.DrawString(font, msg, msgPos + Vector2.One, Color.Black,     0f, Vector2.Zero, 0.85f, SpriteEffects.None, 0f);
        sb.DrawString(font, msg, msgPos, playerWon ? Color.LimeGreen : Color.Red, 0f, Vector2.Zero, 0.85f, SpriteEffects.None, 0f);

        sb.DrawString(font, "Press Space to continue", new Vector2(148, 240), Color.Gray,
            0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
    }
}
