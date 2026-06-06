#if DEBUG
namespace SDK.MonoGame.Tests;

using FluentAssertions;
using Moq;
using SDK.Core.Interfaces;
using SDK.MonoGame.UI;

public class LuaConsoleTests
{
    [Fact]
    public void Toggle_FlipsIsOpen()
    {
        var console = new LuaConsole();
        console.IsOpen.Should().BeFalse();
        console.Toggle();
        console.IsOpen.Should().BeTrue();
        console.Toggle();
        console.IsOpen.Should().BeFalse();
    }

    [Fact]
    public void Append_AccumulatesChars_WhenOpen()
    {
        var console = new LuaConsole();
        console.Toggle();
        console.Append('h');
        console.Append('i');
        console.InputText.Should().Be("hi");
    }

    [Fact]
    public void Append_DoesNothing_WhenClosed()
    {
        var console = new LuaConsole();
        console.Append('x');
        console.InputText.Should().Be(string.Empty);
    }

    [Fact]
    public void Backspace_RemovesLastChar()
    {
        var console = new LuaConsole();
        console.Toggle();
        console.Append('a');
        console.Append('b');
        console.Backspace();
        console.InputText.Should().Be("a");
    }

    [Fact]
    public void Submit_PushesToHistory_ClearsInput()
    {
        var engine = new Mock<IScriptEngine>();
        engine.Setup(e => e.Evaluate<object>(It.IsAny<string>())).Returns("99");

        var console = new LuaConsole();
        console.Toggle();
        console.Append('1');
        console.Append('+');
        console.Append('1');

        console.Submit(engine.Object);

        console.InputText.Should().Be(string.Empty);
        console.History.Should().Contain("> 1+1");
        console.History.Should().Contain("99");
    }

    [Fact]
    public void Submit_Error_AddsErrorToHistory()
    {
        var engine = new Mock<IScriptEngine>();
        engine.Setup(e => e.Evaluate<object>(It.IsAny<string>())).Throws(new Exception("bad"));
        engine.Setup(e => e.Execute(It.IsAny<string>())).Throws(new Exception("bad"));

        var console = new LuaConsole();
        console.Toggle();
        console.Append('?');

        console.Submit(engine.Object);

        console.History.Should().Contain(h => h.Contains("[ERROR] bad"));
        console.InputText.Should().Be(string.Empty);
    }
}
#endif
