namespace SDK.MonoGame.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SDK.Core.Enums;

public class RenderPipeline
{
    private readonly bool _headless;
    private RenderTarget2D? _internalTarget;
    private Effect?         _xbrEffect;
    private Rectangle       _fullscreenRect;

    private static readonly Dictionary<TimeOfDay, Color> DayNightTints = new()
    {
        [TimeOfDay.Morning] = new Color(255, 220, 180),
        [TimeOfDay.Day]     = Color.White,
        [TimeOfDay.Evening] = new Color(255, 180, 100),
        [TimeOfDay.Night]   = new Color(80,  100, 180),
    };

    public RenderPipeline(GraphicsDevice gd, ContentManager content, bool isHeadless)
    {
        _headless = isHeadless;
        if (isHeadless) return;

        _internalTarget = new RenderTarget2D(gd, 480, 270);  // D-14
        _fullscreenRect = new Rectangle(0, 0, 1920, 1080);

        try { _xbrEffect = content.Load<Effect>("Shaders/xBR"); }
        catch { /* shader pas encore compilé MGCB — PointClamp fallback */ }
    }

    public void BeginScene(GraphicsDevice gd)
    {
        if (_headless || _internalTarget is null) return;
        gd.SetRenderTarget(_internalTarget);
        gd.Clear(Color.Black);
    }

    public void EndScene(SpriteBatch sb, TimeOfDay timeOfDay)
    {
        if (_headless || _internalTarget is null) return;

        var gd = sb.GraphicsDevice;
        gd.SetRenderTarget(null);
        gd.Clear(Color.Black);

        var tint = DayNightTints.GetValueOrDefault(timeOfDay, Color.White);

        // Passe 2 : xBR upscale (ou PointClamp passthrough)
        sb.Begin(
            effect:       _xbrEffect,
            samplerState: _xbrEffect is not null ? SamplerState.LinearClamp : SamplerState.PointClamp);
        sb.Draw(_internalTarget, _fullscreenRect, Color.White);
        sb.End();

        // Passe 3 : DayNight tint via SpriteBatch Color (shader stub Plan 03-04)
        if (tint != Color.White)
        {
            sb.Begin(blendState: BlendState.Additive, samplerState: SamplerState.PointClamp);
            sb.Draw(_internalTarget, _fullscreenRect, tint * 0.25f);
            sb.End();
        }
    }

    public RenderTarget2D? InternalTarget => _internalTarget;
}
