namespace SDK.MonoGame.Tests;

using FluentAssertions;
using SDK.MonoGame.UI;

public class DialogueBoxTests
{
    [Fact]
    public void Open_SetsIsOpenTrue_AndCurrentText()
    {
        var box = new DialogueBox();
        box.IsOpen.Should().BeFalse();

        box.Open("Félicitations ! Tu as battu Brock.");

        box.IsOpen.Should().BeTrue();
        box.CurrentText.Should().Be("Félicitations ! Tu as battu Brock.");
    }

    [Fact]
    public void Close_SetsIsOpenFalse()
    {
        var box = new DialogueBox();
        box.Open("Some text");
        box.IsOpen.Should().BeTrue();

        box.Close();

        box.IsOpen.Should().BeFalse();
    }
}
