namespace Application.Services.Authentication.DTOs.Request;

public record ResendEmailConfirmationCodeRequest
{
    public required string Email { get; init; }
}
