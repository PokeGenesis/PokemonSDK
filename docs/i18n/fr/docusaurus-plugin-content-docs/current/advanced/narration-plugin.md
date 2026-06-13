---
sidebar_position: 2
---

# INarrationPlugin

Interface pour les backends TTS personnalisés. Implémentez-la pour ajouter n'importe quel moteur de synthèse vocale à PokemonSDK.

## Interface

```csharp
public interface INarrationPlugin
{
    /// Nom d'affichage du moteur TTS (ex. "Piper", "Windows Speech")
    string EngineName { get; }

    /// True si le moteur est disponible sur la plateforme/machine actuelle
    bool IsSupported { get; }

    /// True pendant qu'un appel SpeakAsync est en cours
    bool IsSpeaking { get; }

    /// Synthétise le texte immédiatement (awaitable)
    Task SpeakAsync(string text, CancellationToken ct = default);

    /// Arrête la parole en cours et vide toute file d'attente interne
    void Stop();

    /// Ajoute le texte à la file de lecture interne
    void Enqueue(string text);
}
```

## Implémenter un backend personnalisé

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

    public void Stop() { /* tuer le processus */ }
    public void Enqueue(string text) => SpeakAsync(text).ConfigureAwait(false);
}
```

## Enregistrer dans le conteneur DI

```csharp
services.AddSingleton<INarrationPlugin, EspeakPlugin>();
services.AddSingleton<NarrationQueue>();
```

## Implémentations incluses

| Classe | Backend | Plateforme |
|--------|---------|-----------|
| `PiperNarrationPlugin` | Binaire Piper TTS | Linux / macOS / Windows |
| `WindowsSpeechPlugin` | API Windows Speech | Windows uniquement |

Voir [SDK.Plugins.TTS](../packages/plugins-tts) pour l'utilisation.
