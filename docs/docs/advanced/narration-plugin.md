---
sidebar_position: 2
---

# INarrationPlugin

Interface for custom TTS backends. Implement it to add any speech synthesis engine to PokemonSDK.

## Interface

```csharp
public interface INarrationPlugin
{
    /// Display name of the TTS engine (e.g. "Piper", "Windows Speech")
    string EngineName { get; }

    /// True if the engine is available on the current platform/machine
    bool IsSupported { get; }

    /// True while a SpeakAsync call is in progress
    bool IsSpeaking { get; }

    /// Speak text immediately (awaitable)
    Task SpeakAsync(string text, CancellationToken ct = default);

    /// Stop current speech and clear any pending queue
    void Stop();

    /// Add text to the internal playback queue
    void Enqueue(string text);
}
```

## Implement a custom backend

```csharp
public class EspeakPlugin : INarrationPlugin
{
    public string EngineName => "espeak-ng";
    public bool IsSupported => File.Exists("/usr/bin/espeak-ng");
    public bool IsSpeaking { get; private set; }

    public async Task SpeakAsync(string text, CancellationToken ct = default)
    {
        IsSpeaking = true;
        var proc = Process.Start("espeak-ng", $"\"{text}\"")!;
        await proc.WaitForExitAsync(ct);
        IsSpeaking = false;
    }

    public void Stop() { /* kill process */ }
    public void Enqueue(string text) => SpeakAsync(text).ConfigureAwait(false);
}
```

## Register in DI

```csharp
services.AddSingleton<INarrationPlugin, EspeakPlugin>();
services.AddSingleton<NarrationQueue>();
```

## Bundled implementations

| Class | Backend | Platform |
|-------|---------|---------|
| `PiperNarrationPlugin` | Piper TTS binary | Linux / macOS / Windows |
| `WindowsSpeechPlugin` | Windows Speech API | Windows only |

See [SDK.Plugins.TTS](../packages/plugins-tts) for usage.
