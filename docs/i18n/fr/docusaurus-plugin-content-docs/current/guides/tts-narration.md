---
sidebar_position: 7
---

# Narration TTS : dialogue vocal

Ce guide montre comment configurer `PiperNarrationPlugin`, vérifier les prérequis, et appeler l'API TTS depuis des scripts Lua.

## Installer

```bash
dotnet add package PokeForge.SDK.Plugins.TTS
```

## Prérequis

`PiperNarrationPlugin` délègue au binaire [Piper](https://github.com/rhasspy/piper) pour la synthèse vocale. Avant d'enregistrer le plugin, assurez-vous que :

| Prérequis | Commande de vérification |
|-----------|--------------------------|
| Binaire `piper` dans le PATH | `piper --version` |
| Lecture audio (`aplay` sur Linux, audio système sur Windows/macOS) | `aplay --version` |

Exécutez le diagnostic intégré :

```bash
pokeforge doctor
```

Cherchez `[OK] TTS: piper found` dans la sortie. Une ligne `[WARN]` ou `[ERROR]` identifie ce qui manque.

## Enregistrer PiperNarrationPlugin

Enregistrez le plugin dans votre racine de composition DI :

```csharp
using PokeForge.SDK.Plugins.TTS;

services.AddSingleton<INarrationPlugin, PiperNarrationPlugin>();
services.AddSingleton<PiperNarrationPlugin>();
```

Le plugin lit sa configuration depuis `appsettings.json` ou des variables d'environnement :

```json
{
  "Narration": {
    "PiperModelPath": "models/en_US-lessac-medium.onnx",
    "SpeakRate": 1.0
  }
}
```

## API Lua

Une fois enregistré, les scripts accèdent au TTS via le global `sdk.tts` :

```lua
-- Prononcer une ligne de dialogue
sdk.tts.speak("Bienvenue à Bourg-Palette, dresseur.")

-- Attendre la fin de la parole, puis continuer
while sdk.tts.is_speaking() do
  coroutine.yield()
end

-- Interrompre en milieu de phrase (ex. : le joueur appuie sur Passer)
sdk.tts.stop()
```

Fonctions disponibles :

| Fonction | Description |
|----------|-------------|
| `sdk.tts.speak(text)` | Met en file le texte pour synthèse et lecture |
| `sdk.tts.stop()` | Interrompt immédiatement la parole en cours |
| `sdk.tts.is_speaking()` | Retourne `true` pendant la lecture audio |

## Pattern SdkGlobals

`sdk.tts` est enregistré via `SdkGlobals`, le point d'extension du SDK pour ajouter des globaux Lua typés :

```csharp
// Câblé en interne par l'enregistrement de PiperNarrationPlugin :
SdkGlobals.Register("tts", new TtsLuaApi(narrationPlugin));
```

Vous pouvez ajouter vos propres globaux de la même façon pour des API Lua personnalisées.

## Backend de narration personnalisé

Pour remplacer Piper par un autre moteur TTS, implémentez `INarrationPlugin` :

```csharp
public interface INarrationPlugin
{
    Task SpeakAsync(string text, CancellationToken ct = default);
    Task StopAsync();
    bool IsSpeaking { get; }
}
```

Voir [API avancées : INarrationPlugin](../advanced/narration-plugin) pour le contrat d'interface complet et les exigences de threading.
