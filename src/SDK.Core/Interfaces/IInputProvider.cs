namespace SDK.Core.Interfaces;

public interface IInputProvider
{
    bool IsActionPressed(string action);
    bool IsActionJustPressed(string action);
}
