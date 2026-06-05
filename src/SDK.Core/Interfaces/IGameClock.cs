namespace SDK.Core.Interfaces;

using SDK.Core.Enums;

public interface IGameClock
{
    TimeOfDay GetTimeOfDay();
    TimeSpan GameElapsed { get; }
    float Speed { get; set; }
    void Update(TimeSpan realDelta);
    void SetGameTime(TimeSpan elapsed);
}
