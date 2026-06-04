namespace SDK.Core.ValueObjects;

public sealed record DamageResult(
    int Damage,
    bool IsCritical,
    decimal TypeMultiplier);
