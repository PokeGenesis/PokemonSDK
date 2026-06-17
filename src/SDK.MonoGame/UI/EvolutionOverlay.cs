namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDK.MonoGame.Input;

public sealed class EvolutionOverlay
{
    private enum EvoPhase { Flashing, Done, Cancelled }

    private EvoPhase    _phase;
    private float       _flashTimer;
    private const float FlashDuration = 2.0f;
    private const float FlashInterval = 0.1f;
    private string      _oldName = "";
    private string      _newName = "";

    public bool IsVisible    { get; private set; }
    public bool IsComplete   { get; private set; }
    public bool WasCancelled { get; private set; }

    public void Trigger(string oldName, string newName)
    {
        _oldName     = oldName;
        _newName     = newName;
        _phase       = EvoPhase.Flashing;
        _flashTimer  = 0f;
        IsVisible    = true;
        IsComplete   = false;
        WasCancelled = false;
    }

    public void Update(KeyboardState ks, KeyboardState prevKs, GameTime gameTime)
    {
        if (!IsVisible) return;
        if (_phase == EvoPhase.Flashing)
        {
            _flashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_flashTimer >= FlashDuration)
                _phase = EvoPhase.Done;
            else if (ks.IsKeyDown(InputMap.Cancel) && !prevKs.IsKeyDown(InputMap.Cancel))
                _phase = EvoPhase.Cancelled;
        }
        else if (_phase == EvoPhase.Done
            && ks.IsKeyDown(InputMap.Confirm) && !prevKs.IsKeyDown(InputMap.Confirm))
        {
            IsVisible    = false;
            IsComplete   = true;
            WasCancelled = false;
        }
        else if (_phase == EvoPhase.Cancelled
            && ks.IsKeyDown(InputMap.Confirm) && !prevKs.IsKeyDown(InputMap.Confirm))
        {
            IsVisible    = false;
            IsComplete   = true;
            WasCancelled = true;
        }
    }

    public void Draw(SpriteBatch sb, Texture2D pixel, SpriteFont? font)
    {
        if (!IsVisible) return;

        if (_phase == EvoPhase.Flashing)
        {
            bool bright = (int)(_flashTimer / FlashInterval) % 2 == 0;
            var bg = bright ? Color.White : new Color(15, 15, 15);
            var fg = bright ? Color.Black : Color.White;
            sb.Draw(pixel, new Rectangle(0, 179, 480, 91), bg);
            if (font != null)
                sb.DrawString(font, $"What? {_oldName} is evolving!",
                    new Vector2(8f, 193f), fg, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
        }
        else if (_phase == EvoPhase.Done)
        {
            sb.Draw(pixel, new Rectangle(0, 179, 480, 91), new Color(15, 15, 15));
            if (font != null)
            {
                sb.DrawString(font, "Congratulations!",
                    new Vector2(8f, 185f), Color.Yellow, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                sb.DrawString(font, $"{_oldName} evolved into {_newName}!",
                    new Vector2(8f, 203f), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                sb.DrawString(font, "Space", new Vector2(432f, 255f), Color.DimGray,
                    0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
            }
        }
        else if (_phase == EvoPhase.Cancelled)
        {
            sb.Draw(pixel, new Rectangle(0, 179, 480, 91), new Color(15, 15, 15));
            if (font != null)
            {
                sb.DrawString(font, $"Oh? {_oldName} stopped evolving!",
                    new Vector2(8f, 193f), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                sb.DrawString(font, "Space", new Vector2(432f, 255f), Color.DimGray,
                    0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
            }
        }
    }
}
