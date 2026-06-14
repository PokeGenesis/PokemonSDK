namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDK.Core.Enums;

public sealed class StatusIcon
{
    private static readonly Dictionary<StatusConditionId, (Color color, string label)> StatusDisplay = new()
    {
        [StatusConditionId.Sleep]     = (Color.CornflowerBlue, "SLP"),
        [StatusConditionId.Freeze]    = (Color.Cyan,           "FRZ"),
        [StatusConditionId.Burn]      = (Color.OrangeRed,      "BRN"),
        [StatusConditionId.Poison]    = (Color.MediumPurple,   "PSN"),
        [StatusConditionId.Paralysis] = (Color.Yellow,         "PRZ"),
    };

    private readonly Texture2D _pixel;

    public StatusIcon(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch sb, StatusConditionId? status, Vector2 position, SpriteFont? font)
    {
        if (status is null or StatusConditionId.None) return;
        if (!StatusDisplay.TryGetValue(status.Value, out var display)) return;

        sb.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, 24, 10), display.color);
        if (font != null)
            sb.DrawString(font, display.label, new Vector2(position.X + 2, position.Y + 1), Color.White, 0f,
                Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
    }

    public void Dispose() => _pixel.Dispose();
}
