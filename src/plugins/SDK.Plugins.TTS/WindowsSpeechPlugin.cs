namespace SDK.Plugins.TTS;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SDK.Core.Interfaces;

public sealed class WindowsSpeechPlugin : INarrationPlugin, IDisposable
{
    private Process? _currentProcess;
    private readonly CancellationTokenSource _cts = new();

    public string EngineName => "WindowsSpeech";
    public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public bool IsSpeaking => _currentProcess is { HasExited: false };

    public async Task SpeakAsync(string text, CancellationToken ct = default)
    {
        if (!IsSupported) return;

        var script = $"Add-Type -AssemblyName System.speech; (New-Object System.Speech.Synthesis.SpeechSynthesizer).Speak('{text.Replace("'", "''")}')";
        var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        var psi = new ProcessStartInfo("powershell.exe")
        {
            Arguments = $"-NoProfile -EncodedCommand {encoded}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        _currentProcess = Process.Start(psi);
        if (_currentProcess != null)
            await _currentProcess.WaitForExitAsync(ct);
    }

    public void Stop()
    {
        try { _currentProcess?.Kill(); } catch { }
        _cts.Cancel();
    }

    public void Enqueue(string text) => _ = SpeakAsync(text);

    public void Dispose()
    {
        Stop();
        _currentProcess?.Dispose();
        _cts.Dispose();
    }
}
