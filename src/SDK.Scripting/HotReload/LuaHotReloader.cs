#if DEBUG
namespace SDK.Scripting.HotReload;

using SDK.Core.Interfaces;

public sealed class LuaHotReloader : IDisposable
{
    private readonly IScriptEngine _engine;
    private readonly FileSystemWatcher _watcher;

    public event Action<string, string>? OnReloadError;
    public string? LastError { get; private set; }

    public LuaHotReloader(string directory, IScriptEngine engine)
    {
        _engine  = engine;
        _watcher = new FileSystemWatcher(directory, "*.lua")
        {
            NotifyFilter          = NotifyFilters.LastWrite,
            EnableRaisingEvents   = true,
            IncludeSubdirectories = true,
        };
        _watcher.Changed += OnChanged;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            _engine.Reload(e.FullPath);
            LastError = null;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            OnReloadError?.Invoke(e.FullPath, ex.Message);
        }
    }

    public void Dispose()
    {
        _watcher.Changed -= OnChanged;
        _watcher.Dispose();
    }
}
#endif
