namespace Overclocked.Contracts.Authentication;

public record ResendEmailConfirmationCodeRequest
{
    public required string Email { get; init; }
}
