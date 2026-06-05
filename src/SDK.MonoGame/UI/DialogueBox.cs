namespace SDK.MonoGame.UI;

public class DialogueBox
{
    public bool IsOpen { get; private set; }
    public string CurrentText { get; private set; } = string.Empty;

    public void Open(string text)
    {
        CurrentText = text;
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    // Draw stub — SpriteFont/MGCB compilation déférée Phase 7 DX
    public void Draw() { }
}
