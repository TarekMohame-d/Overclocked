namespace Overclocked.Application.Authentication.Commands.Register;

public record RegisterCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
}
