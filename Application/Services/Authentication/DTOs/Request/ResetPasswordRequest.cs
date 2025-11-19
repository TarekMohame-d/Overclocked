namespace Application.Services.Authentication.DTOs.Request;

public record ResetPasswordRequest
{
    public required string Email { get; init; }
    public required string Code { get; init; }
    public required string Password { get; init; }
}
