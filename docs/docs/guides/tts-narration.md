---
sidebar_position: 7
---

# TTS Narration: Voice Dialogue

This guide shows how to set up `PiperNarrationPlugin`, verify prerequisites, and call the TTS API from Lua scripts.

## Install

```bash
dotnet add package PokeForge.SDK.Plugins.TTS
```

## Prerequisites

`PiperNarrationPlugin` shells out to the [Piper](https://github.com/rhasspy/piper) binary for text-to-speech synthesis. Before registering the plugin, ensure:

| Requirement | Check command |
|-------------|---------------|
| `piper` binary on PATH | `piper --version` |
| Audio playback (`aplay` on Linux, system audio on Windows/macOS) | `aplay --version` |

Run the built-in doctor check:

```bash
pokeforge doctor
```

Look for `[OK] TTS: piper found` in the output. A `[WARN]` or `[ERROR]` line identifies what is missing.

## Register PiperNarrationPlugin

Register the plugin in your DI composition root:

```csharp
using PokeForge.SDK.Plugins.TTS;

services.AddSingleton<INarrationPlugin, PiperNarrationPlugin>();
services.AddSingleton<PiperNarrationPlugin>();
```

The plugin reads its configuration from `appsettings.json` or environment variables:

```json
{
  "Narration": {
    "PiperModelPath": "models/en_US-lessac-medium.onnx",
    "SpeakRate": 1.0
  }
}
```

## Lua API

Once registered, scripts access TTS via the `sdk.tts` global:

```lua
-- Speak a line of dialogue
sdk.tts.speak("Welcome to Pallet Town, trainer.")

-- Wait until speech finishes, then continue
while sdk.tts.is_speaking() do
  coroutine.yield()
end

-- Stop mid-sentence (e.g., player pressed Skip)
sdk.tts.stop()
```

Available functions:

| Function | Description |
|----------|-------------|
| `sdk.tts.speak(text)` | Queue text for synthesis and playback |
| `sdk.tts.stop()` | Interrupt current speech immediately |
| `sdk.tts.is_speaking()` | Returns `true` while audio is playing |

## SdkGlobals Pattern

`sdk.tts` is registered via `SdkGlobals`, the SDK's extension point for adding typed Lua globals:

```csharp
// Internally wired by PiperNarrationPlugin registration:
SdkGlobals.Register("tts", new TtsLuaApi(narrationPlugin));
```

You can add your own globals the same way for custom Lua APIs.

## Custom Narration Backend

To replace Piper with a different TTS engine, implement `INarrationPlugin`:

```csharp
public interface INarrationPlugin
{
    Task SpeakAsync(string text, CancellationToken ct = default);
    Task StopAsync();
    bool IsSpeaking { get; }
}
```

See [Advanced APIs: INarrationPlugin](../advanced/narration-plugin) for the full interface contract and threading requirements.
