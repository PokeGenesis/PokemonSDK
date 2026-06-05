namespace SDK.Scripting.Bindings;

using SDK.Core.ValueObjects;

public class BadgeApi
{
    private GameState _state;

    public BadgeApi(GameState initial) => _state = initial;

    public void AwardBadge(string id) => _state = _state.WithFlag($"badge_{id}", true);

    public bool HasBadge(string id) => _state.GetFlag<bool>($"badge_{id}");

    public GameState GetState() => _state;
}
