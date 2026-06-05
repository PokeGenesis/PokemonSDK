namespace SDK.Core.Services;

using SDK.Core.Enums;
using SDK.Core.Interfaces;

public class RealTimeClock(Func<DateTime>? clock = null) : IRealTimeClock
{
    private readonly Func<DateTime> _clock = clock ?? (() => DateTime.UtcNow);

    public TimeOfDay GetTimeOfDay() => MapHour(_clock().Hour);

    internal static TimeOfDay MapHour(int hour) => hour switch
    {
        >= 6 and < 11  => TimeOfDay.Morning,
        >= 11 and < 17 => TimeOfDay.Day,
        >= 17 and < 21 => TimeOfDay.Evening,
        _              => TimeOfDay.Night,
    };
}
