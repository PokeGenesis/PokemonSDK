namespace SDK.Plugins.TTS;

using System.Threading.Channels;
using SDK.Core.Interfaces;

public sealed class NarrationQueue : IDisposable
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _processingTask;
    private readonly INarrationPlugin _plugin;

    public NarrationQueue(INarrationPlugin plugin)
    {
        _plugin = plugin;
        _processingTask = Task.Run(() => ProcessAsync(_cts.Token));
    }

    public void Enqueue(string text) => _channel.Writer.TryWrite(text);

    public void Stop()
    {
        _cts.Cancel();
        _channel.Writer.TryComplete();
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var text in _channel.Reader.ReadAllAsync(ct))
                await _plugin.SpeakAsync(text, ct);
        }
        catch (OperationCanceledException) { }
    }

    public void Dispose()
    {
        Stop();
        try { _processingTask.Wait(TimeSpan.FromSeconds(2)); } catch { }
        _cts.Dispose();
    }
}
