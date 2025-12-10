namespace Overclocked.Contracts.Authentication;

public record ConfirmEmailRequest
{
    public required string Email { get; init; }
    public required string Code { get; init; }
}
