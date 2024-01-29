namespace Play.Common.Settings;

public class AuthSettings
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required IEnumerable<string> Audience { get; init; }
    public required int ExpirationTimeSeconds { get; init; }
}