namespace SDK.Plugins.TTS.Tests;

using FluentAssertions;

public class PiperTtsPluginTests
{
    [Fact]
    public void EngineName_IsPiperTTS()
    {
        var sut = new PiperTtsPlugin("/nonexistent/piper", "");
        sut.EngineName.Should().Be("PiperTTS");
    }

    [Fact]
    public void IsSupported_False_WhenPiperAbsent()
    {
        var sut = new PiperTtsPlugin("/nonexistent/piper", "");
        sut.IsSupported.Should().BeFalse();
    }

    [Fact]
    public void IsSpeaking_False_WhenIdle()
    {
        var sut = new PiperTtsPlugin("/nonexistent/piper", "");
        sut.IsSpeaking.Should().BeFalse();
    }

    [Fact]
    public async Task SpeakAsync_NoOp_WhenNotSupported()
    {
        var sut = new PiperTtsPlugin("/nonexistent/piper", "");
        await sut.Invoking(p => p.SpeakAsync("test")).Should().NotThrowAsync();
    }

    [Fact]
    public async Task SpeakAsync_WithCancelledToken_DoesNotBlock()
    {
        var sut = new PiperTtsPlugin("/nonexistent/piper", "");
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await sut.Invoking(p => p.SpeakAsync("test", cts.Token)).Should().NotThrowAsync();
    }

    [Fact]
    public void Stop_DoesNotThrow()
    {
        var sut = new PiperTtsPlugin("/nonexistent/piper", "");
        sut.Invoking(p => p.Stop()).Should().NotThrow();
    }
}
