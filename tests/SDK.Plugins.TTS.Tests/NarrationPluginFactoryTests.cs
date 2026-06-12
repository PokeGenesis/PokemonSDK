namespace SDK.Plugins.TTS.Tests;

using FluentAssertions;

public class NarrationPluginFactoryTests
{
    [Fact]
    public void CreateForCurrentPlatform_ReturnsNonNull()
        => NarrationPluginFactory.CreateForCurrentPlatform().Should().NotBeNull();

    [Fact]
    public void CreateForCurrentPlatform_NullOptions_DoesNotThrow()
        => ((Action)(() => NarrationPluginFactory.CreateForCurrentPlatform(null)))
           .Should().NotThrow();

    [Fact]
    public void CreateForCurrentPlatform_IsSupported()
    {
        var plugin = NarrationPluginFactory.CreateForCurrentPlatform();
        plugin.IsSupported.Should().BeTrue();
    }

    [Fact]
    public void CreateForCurrentPlatform_PiperAbsent_FallsBackToSupported()
    {
        var plugin = NarrationPluginFactory.CreateForCurrentPlatform(
            new NarrationOptions("/nonexistent/piper", ""));
        plugin.Should().NotBeNull();
        plugin.IsSupported.Should().BeTrue();
    }
}
