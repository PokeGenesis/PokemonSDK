namespace SDK.Plugins.TTS.Tests;

using System.Diagnostics;
using FluentAssertions;
using Moq;
using SDK.Core.Interfaces;

public class NarrationQueueTests
{
    [Fact]
    public void Enqueue_ReturnsImmediately()
    {
        var mock = new Mock<INarrationPlugin>();
        mock.Setup(p => p.SpeakAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Delay(200));

        using var q = new NarrationQueue(mock.Object);
        var sw = Stopwatch.StartNew();
        q.Enqueue("text1");
        q.Enqueue("text2");
        sw.Stop();
        sw.ElapsedMilliseconds.Should().BeLessThan(50);
    }

    [Fact]
    public async Task Queue_ProcessesItems()
    {
        var count = 0;
        var mock = new Mock<INarrationPlugin>();
        mock.Setup(p => p.SpeakAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((_, _) => count++)
            .Returns(Task.CompletedTask);

        using var q = new NarrationQueue(mock.Object);
        q.Enqueue("a");
        q.Enqueue("b");
        await Task.Delay(100);
        count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void Stop_DoesNotBlock()
    {
        var mock = new Mock<INarrationPlugin>();
        mock.Setup(p => p.SpeakAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Delay(500));

        using var q = new NarrationQueue(mock.Object);
        var sw = Stopwatch.StartNew();
        q.Stop();
        sw.Stop();
        sw.ElapsedMilliseconds.Should().BeLessThan(50);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var mock = new Mock<INarrationPlugin>();
        mock.Setup(p => p.SpeakAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var q = new NarrationQueue(mock.Object);
        q.Invoking(x => x.Dispose()).Should().NotThrow();
    }
}
