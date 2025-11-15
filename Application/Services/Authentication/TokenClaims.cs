namespace Application.Services.Authentication;

public record TokenClaims
{
    public required string Email { get; init; }
    public required int RoleId { get; init; }
    public required string DeviceId { get; init; }
    public required string UserId { get; init; }
    public required IEnumerable<string> Permissions { get; init; } = [];
}
