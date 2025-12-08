namespace Overclocked.Contracts.Authentication;

public record LoginRequest(string Email, string Password, string DeviceId);
