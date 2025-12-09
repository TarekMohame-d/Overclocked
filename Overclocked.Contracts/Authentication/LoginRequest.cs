namespace Overclocked.Contracts.Authentication;

public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string DeviceId { get; init; }
}
