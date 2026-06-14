namespace SDK.MonoGame.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public sealed class WipeTransition
{
    private readonly int _duration;
    private readonly Action _onComplete;
    private readonly Texture2D _pixel;
    private int _frame;
    private bool _done;

    public bool IsComplete => _done;

    public WipeTransition(GraphicsDevice gd, int duration, Action onComplete)
    {
        _duration = duration;
        _onComplete = onComplete;
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Update()
    {
        if (_done) return;
        _frame++;
        if (_frame >= _duration) { _done = true; _onComplete(); }
    }

    public void Draw(SpriteBatch sb, int screenW = 480, int screenH = 270)
    {
        float alpha = Math.Min(1f, (float)_frame / _duration);
        sb.Draw(_pixel, new Rectangle(0, 0, screenW, screenH), Color.Black * alpha);
    }

    public void Dispose() => _pixel.Dispose();
}
