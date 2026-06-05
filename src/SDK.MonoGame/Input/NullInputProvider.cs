namespace SDK.MonoGame.Input;

using SDK.Core.Interfaces;

public class NullInputProvider : IInputProvider
{
    public bool IsActionPressed(string action)     => false;
    public bool IsActionJustPressed(string action) => false;
}
