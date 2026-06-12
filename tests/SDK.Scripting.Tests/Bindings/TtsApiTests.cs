namespace SDK.Scripting.Tests.Bindings;

using FluentAssertions;
using Moq;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using SDK.Scripting.Bindings;
using SDK.Scripting.Engine;

public class TtsApiTests
{
    [Fact]
    public void Speak_DelegatesTo_Enqueue()
    {
        var mock = new Mock<INarrationPlugin>();
        var api = new TtsApi(mock.Object);

        api.speak("hello");

        mock.Verify(p => p.Enqueue("hello"), Times.Once());
    }

    [Fact]
    public void Stop_DelegatesTo_Stop()
    {
        var mock = new Mock<INarrationPlugin>();
        var api = new TtsApi(mock.Object);

        api.stop();

        mock.Verify(p => p.Stop(), Times.Once());
    }

    [Fact]
    public void NpcInteractionRunner_LuaTtsSpeak_DelegatesEnqueue()
    {
        var mock = new Mock<INarrationPlugin>();
        var engine = new LuaScriptEngine();
        var state = new GameState();

        NpcInteractionRunner.Run(engine, state, "sdk.tts.speak('bonjour')", mock.Object);

        mock.Verify(p => p.Enqueue("bonjour"), Times.Once());
    }
}
