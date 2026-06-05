namespace SDK.Core.Services;

using SDK.Core.Enums;
using SDK.Core.Interfaces;

public class GameTimeClock : IGameClock
{
    private TimeSpan _elapsed = TimeSpan.Zero;

    public float Speed { get; set; } = 1f / 60f;
    public TimeSpan GameElapsed => _elapsed;

    public void Update(TimeSpan realDelta)
        => _elapsed += TimeSpan.FromMinutes(realDelta.TotalSeconds * Speed);

    public void SetGameTime(TimeSpan elapsed) => _elapsed = elapsed;

    public TimeOfDay GetTimeOfDay()
    {
        int gameHour = (int)(_elapsed.TotalHours % 24);
        return RealTimeClock.MapHour(gameHour);
    }
}
