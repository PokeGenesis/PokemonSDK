---
sidebar_position: 9
---

# PokeForge.SDK.Plugins.TTS

Text-to-speech narration system — `INarrationPlugin` interface, a queued narrator, and two ready-made backends.

```bash
dotnet add package PokeForge.SDK.Plugins.TTS
```

## INarrationPlugin

The core interface. Implement it to add any TTS backend.

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

## NarrationQueue

`NarrationQueue` wraps any `INarrationPlugin` and serializes speech requests. Use it to narrate dialog lines without overlap:

```csharp
var tts = new PiperNarrationPlugin(modelPath: "voices/en_US-amy-medium.onnx");
var queue = new NarrationQueue(tts);

queue.Enqueue("Welcome to the world of Pokémon!");
queue.Enqueue("Professor Oak will guide you.");
// Lines play back-to-back, no overlap
```

Stop narration mid-queue:

```csharp
queue.Stop(); // cancels current line, clears pending
```

## PiperNarrationPlugin

Cross-platform backend using the [Piper TTS](https://github.com/rhasspy/piper) binary. Runs on Linux, macOS, and Windows.

```csharp
var plugin = new PiperNarrationPlugin(
    modelPath: "voices/en_US-amy-medium.onnx",
    piperExecutable: "piper"); // must be on PATH or provide absolute path

Console.WriteLine(plugin.IsSupported); // false if piper binary not found
await plugin.SpeakAsync("Pikachu used Thunderbolt!");
```

Check availability before use — `IsSupported` returns `false` if the binary is missing.

## WindowsSpeechPlugin

Windows-only backend using the built-in Windows Speech API. No installation required.

```csharp
var plugin = new WindowsSpeechPlugin();
// IsSupported = true on Windows, false on Linux/macOS

await plugin.SpeakAsync("A wild Rattata appeared!");
```

## Choosing a backend

```csharp
INarrationPlugin tts = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? new WindowsSpeechPlugin()
    : new PiperNarrationPlugin("voices/en_US-amy-medium.onnx");

var queue = new NarrationQueue(tts);
```

Or register conditionally in DI:

```csharp
services.AddSingleton<INarrationPlugin>(sp =>
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? new WindowsSpeechPlugin()
        : new PiperNarrationPlugin("voices/en_US-amy-medium.onnx"));

services.AddSingleton<NarrationQueue>();
```
