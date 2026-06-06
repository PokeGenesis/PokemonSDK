#if DEBUG
using FluentAssertions;
using Moq;
using SDK.Core.Interfaces;
using SDK.Scripting.HotReload;
using Xunit;

namespace SDK.Scripting.Tests;

public class LuaHotReloaderTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public LuaHotReloaderTests() => Directory.CreateDirectory(_tempDir);

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private string WriteLua(string name, string content = "x = 1")
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void FileChanged_TriggersReload()
    {
        var mock = new Mock<IScriptEngine>();
        using var reloader = new LuaHotReloader(_tempDir, mock.Object);

        var gate = new ManualResetEventSlim(false);
        mock.Setup(e => e.Reload(It.IsAny<string>())).Callback<string>(_ => gate.Set());

        WriteLua("test.lua");

        gate.Wait(timeout: TimeSpan.FromMilliseconds(800));

        mock.Verify(e => e.Reload(It.IsAny<string>()), Times.AtLeastOnce());
    }

    [Fact]
    public void NonLuaFile_IsIgnored()
    {
        var mock = new Mock<IScriptEngine>();
        using var reloader = new LuaHotReloader(_tempDir, mock.Object);

        File.WriteAllText(Path.Combine(_tempDir, "readme.txt"), "hello");
        // Attente courte — aucun event attendu
        Thread.Sleep(300);

        mock.Verify(e => e.Reload(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public void ReloadError_RaisesOnReloadError()
    {
        var mock = new Mock<IScriptEngine>();
        mock.Setup(e => e.Reload(It.IsAny<string>())).Throws(new Exception("bad syntax"));

        using var reloader = new LuaHotReloader(_tempDir, mock.Object);

        string? capturedPath    = null;
        string? capturedMessage = null;
        var gate = new ManualResetEventSlim(false);

        reloader.OnReloadError += (path, msg) =>
        {
            capturedPath    = path;
            capturedMessage = msg;
            gate.Set();
        };

        WriteLua("broken.lua", "???");

        gate.Wait(timeout: TimeSpan.FromMilliseconds(800));

        capturedMessage.Should().Be("bad syntax");
        reloader.LastError.Should().Be("bad syntax");
    }

    [Fact]
    public void Dispose_StopsWatcher()
    {
        var mock = new Mock<IScriptEngine>();
        var reloader = new LuaHotReloader(_tempDir, mock.Object);
        reloader.Dispose();

        // Écrire après Dispose — Reload ne doit pas être appelé
        WriteLua("after_dispose.lua");
        Thread.Sleep(300);

        mock.Verify(e => e.Reload(It.IsAny<string>()), Times.Never());
    }
}
#endif
