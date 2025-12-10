namespace Overclocked.Contracts.Authentication;

public record ForgetPasswordRequest
{
    public required string Email { get; init; }
}
