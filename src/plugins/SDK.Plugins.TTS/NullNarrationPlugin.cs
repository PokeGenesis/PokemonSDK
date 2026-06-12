namespace SDK.Plugins.TTS;

using SDK.Core.Interfaces;

public sealed class NullNarrationPlugin : INarrationPlugin
{
    public string EngineName => "Null";
    public bool IsSupported => true;
    public bool IsSpeaking => false;

    public Task SpeakAsync(string text, CancellationToken ct = default) => Task.CompletedTask;
    public void Stop() { }
    public void Enqueue(string text) { }
}
