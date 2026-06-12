namespace SDK.Plugins.TTS;

using System.Runtime.InteropServices;
using SDK.Core.Interfaces;

public record NarrationOptions(string? PiperPath = null, string? VoiceModel = null);

public static class NarrationPluginFactory
{
    public static INarrationPlugin CreateForCurrentPlatform(NarrationOptions? options = null)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var win = new WindowsSpeechPlugin();
            if (win.IsSupported) return win;
        }

        var piper = new PiperTtsPlugin(
            options?.PiperPath ?? "piper",
            options?.VoiceModel ?? "");
        if (piper.IsSupported) return piper;

        return new NullNarrationPlugin();
    }
}
