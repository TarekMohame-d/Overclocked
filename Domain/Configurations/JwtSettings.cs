namespace Domain.Configurations;

public sealed class JwtSettings
{
    public required string SigningKey { get; init; }
    public int ExpiresInMinutes { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
}
