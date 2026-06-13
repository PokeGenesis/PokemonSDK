---
sidebar_position: 2
---

# INarrationPlugin

Interface for TTS narration engines. Implement to add custom speech synthesis.

```csharp
public interface INarrationPlugin
{
    string EngineName { get; }
    bool IsSupported { get; }
    bool IsSpeaking { get; }
    Task SpeakAsync(string text, CancellationToken ct = default);
    void Stop();
    void Enqueue(string text);
}
```
