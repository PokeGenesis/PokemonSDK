namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public sealed class ExpBar
{
    private readonly Texture2D _pixel;
    private float _displayRatio = -1f;

    public ExpBar(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Update(GameTime gameTime, int currentExp, int nextLevelExp)
    {
        float target = nextLevelExp > 0
            ? Math.Clamp((float)currentExp / nextLevelExp, 0f, 1f)
            : 1f;

        if (_displayRatio < 0f)
        {
            _displayRatio = target;
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float diff = target - _displayRatio;
        if (MathF.Abs(diff) < 0.002f)
            _displayRatio = target;
        else
            _displayRatio += diff * Math.Min(dt * 5f, 1f);
    }

    public void Draw(SpriteBatch sb, int level, Vector2 position, int width, int height, SpriteFont? font)
    {
        float ratio = _displayRatio >= 0f ? _displayRatio : 0f;

        sb.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, width, height), Color.DarkGray);
        sb.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, (int)(width * ratio), height),
            level >= 100 ? Color.Gold : Color.Yellow);

        if (font != null)
            sb.DrawString(font, level >= 100 ? "Lv.MAX" : $"Lv.{level}",
                new Vector2(position.X, position.Y - 11),
                Color.White, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
    }

    public void Dispose() => _pixel.Dispose();
}
