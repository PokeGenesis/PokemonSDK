#if DEBUG
namespace SDK.MonoGame.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDK.Core.Interfaces;

public sealed class LuaConsole
{
    private readonly List<string> _history = new();
    private string _input = string.Empty;

    public bool IsOpen { get; private set; }
    public string InputText => _input;
    public IReadOnlyList<string> History => _history;

    public void Toggle() => IsOpen = !IsOpen;

    public void Append(char c)
    {
        if (!IsOpen) return;
        _input += c;
    }

    public void Backspace()
    {
        if (!IsOpen || _input.Length == 0) return;
        _input = _input[..^1];
    }

    public void Submit(IScriptEngine engine)
    {
        if (!IsOpen || _input.Length == 0) return;
        var cmd = _input;
        _input = string.Empty;
        try
        {
            var expr = cmd.TrimStart().StartsWith("return ", StringComparison.OrdinalIgnoreCase)
                ? cmd
                : $"return ({cmd})";
            var result = engine.Evaluate<object>(expr);
            _history.Add($"> {cmd}");
            _history.Add(result?.ToString() ?? "nil");
        }
        catch
        {
            try
            {
                engine.Execute(cmd);
                _history.Add($"> {cmd}");
                _history.Add("ok");
            }
            catch (Exception ex)
            {
                _history.Add($"> {cmd}");
                _history.Add($"[ERROR] {ex.Message}");
            }
        }
        if (_history.Count > 40) _history.RemoveRange(0, _history.Count - 40);
    }

    public void Draw(SpriteBatch sb, SpriteFont? font)
    {
        if (!IsOpen || font is null) return;
        var overlay = new Texture2D(sb.GraphicsDevice, 1, 1);
        overlay.SetData(new[] { new Color(0, 0, 0, 180) });
        sb.Begin();
        sb.Draw(overlay, new Rectangle(0, 600, 1920, 480), Color.White);
        int y = 610;
        foreach (var line in _history.TakeLast(10))
        {
            sb.DrawString(font, line, new Vector2(20, y), Color.White);
            y += 22;
        }
        sb.DrawString(font, $"> {_input}_", new Vector2(20, y + 4), Color.Yellow);
        sb.End();
    }
}
#endif
