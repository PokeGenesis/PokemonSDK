namespace SDK.MonoGame.Input;

using Microsoft.Xna.Framework.Input;
using SDK.Core.Interfaces;

public class KeyboardInputProvider : IInputProvider
{
    private KeyboardState _current;
    private KeyboardState _previous;

    public void Update()
    {
        _previous = _current;
        _current  = Keyboard.GetState();
    }

    public bool IsActionPressed(string action) => action switch
    {
        "Up"    => _current.IsKeyDown(Keys.Up)    || _current.IsKeyDown(Keys.W),
        "Down"  => _current.IsKeyDown(Keys.Down)  || _current.IsKeyDown(Keys.S),
        "Left"  => _current.IsKeyDown(Keys.Left)  || _current.IsKeyDown(Keys.A),
        "Right" => _current.IsKeyDown(Keys.Right) || _current.IsKeyDown(Keys.D),
        "A"     => _current.IsKeyDown(Keys.Z)     || _current.IsKeyDown(Keys.Enter),
        "B"     => _current.IsKeyDown(Keys.X)     || _current.IsKeyDown(Keys.Back),
        "Start" => _current.IsKeyDown(Keys.Escape),
        _       => false,
    };

    public bool IsActionJustPressed(string action) => action switch
    {
        "Up"    => WasJust(Keys.Up)    || WasJust(Keys.W),
        "Down"  => WasJust(Keys.Down)  || WasJust(Keys.S),
        "Left"  => WasJust(Keys.Left)  || WasJust(Keys.A),
        "Right" => WasJust(Keys.Right) || WasJust(Keys.D),
        "A"     => WasJust(Keys.Z)     || WasJust(Keys.Enter),
        "B"     => WasJust(Keys.X)     || WasJust(Keys.Back),
        "Start" => WasJust(Keys.Escape),
        _       => false,
    };

    private bool WasJust(Keys key)
        => _current.IsKeyDown(key) && !_previous.IsKeyDown(key);
}
