namespace SDK.Tools.Validation;

public enum SeverityLevel { Ok, Warn, Error }

public sealed record SpriteValidationResult(
    SpriteEntry Entry,
    SeverityLevel Severity,
    string Message
);
