namespace SDK.Core.Interfaces;

public interface INarrationPlugin
{
    string EngineName { get; }
    bool IsSupported { get; }
    bool IsSpeaking { get; }
    Task SpeakAsync(string text, CancellationToken ct = default);
    void Stop();
    void Enqueue(string text);
}
