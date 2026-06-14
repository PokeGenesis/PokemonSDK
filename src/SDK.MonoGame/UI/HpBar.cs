namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public sealed class HpBar
{
    private readonly Texture2D _pixel;

    public HpBar(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch sb, int currentHp, int maxHp, Vector2 position,
                     int barWidth, int barHeight, string label, SpriteFont? font)
    {
        float ratio = maxHp > 0 ? (float)currentHp / maxHp : 0f;
        var color = ratio > 0.5f ? Color.Green
                  : ratio > 0.2f ? Color.Yellow
                  : Color.Red;

        sb.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, barWidth, barHeight), Color.DarkGray);
        sb.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, (int)(barWidth * ratio), barHeight), color);

        if (font != null)
            sb.DrawString(font, $"{label} {currentHp}/{maxHp}",
                new Vector2(position.X, position.Y - 12), Color.White);
    }

    public void Dispose() => _pixel.Dispose();
}
