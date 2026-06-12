namespace SDK.Plugins.TTS;

using System.Diagnostics;
using SDK.Core.Interfaces;

public sealed class PiperTtsPlugin : INarrationPlugin, IDisposable
{
    private readonly string _piperPath;
    private readonly string _voiceModel;
    private Process? _currentProcess;

    public PiperTtsPlugin(string piperPath, string voiceModel)
    {
        _piperPath = piperPath;
        _voiceModel = voiceModel;
    }

    public string EngineName => "PiperTTS";
    public bool IsSupported => File.Exists(_piperPath) || IsOnPath(_piperPath);
    public bool IsSpeaking => _currentProcess is { HasExited: false };

    public async Task SpeakAsync(string text, CancellationToken ct = default)
    {
        if (!IsSupported) return;

        var inputFile = Path.GetTempFileName();
        var outputFile = Path.ChangeExtension(inputFile, ".wav");
        try
        {
            await File.WriteAllTextAsync(inputFile, text, ct);

            using var piperProc = Process.Start(new ProcessStartInfo(_piperPath)
            {
                Arguments = $"--input \"{inputFile}\" --output_file \"{outputFile}\" --model \"{_voiceModel}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            })!;
            await piperProc.WaitForExitAsync(ct);
            if (piperProc.ExitCode != 0 || !File.Exists(outputFile)) return;

            var player = DeterminePlayer();
            if (player == null) return;

            _currentProcess = Process.Start(new ProcessStartInfo(player)
            {
                Arguments = $"\"{outputFile}\"",
                UseShellExecute = false
            });
            if (_currentProcess != null)
                await _currentProcess.WaitForExitAsync(ct);
        }
        finally
        {
            try { File.Delete(inputFile); } catch { }
            try { File.Delete(outputFile); } catch { }
        }
    }

    public void Stop()
    {
        try { _currentProcess?.Kill(); } catch { }
    }

    public void Enqueue(string text) => _ = SpeakAsync(text);

    public void Dispose()
    {
        Stop();
        _currentProcess?.Dispose();
    }

    private static bool IsOnPath(string binary)
    {
        try
        {
            var pathDirs = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
            return pathDirs.Any(dir => File.Exists(Path.Combine(dir, binary)));
        }
        catch { return false; }
    }

    private static string? DeterminePlayer()
    {
        var players = new[] { "aplay", "paplay", "mplayer" };
        var pathDirs = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
        foreach (var player in players)
        {
            if (pathDirs.Any(dir => File.Exists(Path.Combine(dir, player))))
                return player;
        }
        return null;
    }
}
