namespace SDK.Core.Tests;

using FluentAssertions;
using SDK.Core.Services;
using SDK.Core.ValueObjects;

public class SaveSystemTests
{
    private readonly SaveSystem _sut = new();

    [Fact]
    public void Save_Load_RoundTrip_PreservesAllFields()
    {
        var path = Path.GetTempFileName();
        try
        {
            var state = new GameState
            {
                PlayerName = "Ash",
                PlaytimeSeconds = 3600
            }.WithFlag("badge_boulder", true);

            _sut.Save(state, path);
            var loaded = _sut.Load(path);

            loaded.Should().NotBeNull();
            loaded!.PlayerName.Should().Be("Ash");
            loaded.PlaytimeSeconds.Should().Be(3600);
            loaded.GetFlag<bool>("badge_boulder").Should().BeTrue();
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Load_ReturnsNull_WhenFileNotFound()
    {
        var result = _sut.Load("/tmp/does_not_exist_xyz_12345.json");
        result.Should().BeNull();
    }

    [Fact]
    public void Save_Overwrites_PreviousSave()
    {
        var path = Path.GetTempFileName();
        try
        {
            var first = new GameState { PlayerName = "Ash", PlaytimeSeconds = 100 };
            var second = new GameState { PlayerName = "Misty", PlaytimeSeconds = 200 };

            _sut.Save(first, path);
            _sut.Save(second, path);
            var loaded = _sut.Load(path);

            loaded!.PlayerName.Should().Be("Misty");
            loaded.PlaytimeSeconds.Should().Be(200);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Load_PreservesFlags_D12()
    {
        var path = Path.GetTempFileName();
        try
        {
            var state = new GameState { PlayerName = "Gary" }
                .WithFlag("badge_boulder", true)
                .WithFlag("badge_cascade", false)
                .WithFlag("badge_thunder", true);

            _sut.Save(state, path);
            var loaded = _sut.Load(path);

            loaded!.GetFlag<bool>("badge_boulder").Should().BeTrue();
            loaded.GetFlag<bool>("badge_cascade").Should().BeFalse();
            loaded.GetFlag<bool>("badge_thunder").Should().BeTrue();
        }
        finally { File.Delete(path); }
    }
}
