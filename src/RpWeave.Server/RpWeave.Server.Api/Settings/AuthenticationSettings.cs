namespace RpWeave.Server.Api.Settings;

public class AuthenticationSettings
{
    public required string TokenSecret { get; init; }
    public required string RefreshTokenSecret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
}