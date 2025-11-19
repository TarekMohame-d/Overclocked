namespace Application.Services.Authentication.DTOs.Request;

public record ForgetPasswordRequest
{
    public required string Email { get; init; }
}
