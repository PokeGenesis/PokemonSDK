namespace SDK.Plugins.TTS.Tests;

using FluentAssertions;

public class NullNarrationPluginTests
{
    private readonly NullNarrationPlugin _sut = new();

    [Fact]
    public void EngineName_IsNull()
        => _sut.EngineName.Should().Be("Null");

    [Fact]
    public void IsSupported_ReturnsTrue()
        => _sut.IsSupported.Should().BeTrue();

    [Fact]
    public void IsSpeaking_ReturnsFalse()
        => _sut.IsSpeaking.Should().BeFalse();

    [Fact]
    public async Task SpeakAsync_Completes_WithoutException()
        => await _sut.Invoking(p => p.SpeakAsync("test")).Should().NotThrowAsync();

    [Fact]
    public void Stop_DoesNotThrow()
        => _sut.Invoking(p => p.Stop()).Should().NotThrow();

    [Fact]
    public void Enqueue_DoesNotThrow()
        => _sut.Invoking(p => p.Enqueue("test")).Should().NotThrow();
}
