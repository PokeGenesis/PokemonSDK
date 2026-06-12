namespace SDK.Scripting.Bindings;

using SDK.Core.Interfaces;

public class TtsApi
{
    private readonly INarrationPlugin _plugin;

    public TtsApi(INarrationPlugin plugin) => _plugin = plugin;

    public void speak(string text) => _plugin.Enqueue(text);

    public void stop() => _plugin.Stop();

    public bool is_speaking() => _plugin.IsSpeaking;
}
