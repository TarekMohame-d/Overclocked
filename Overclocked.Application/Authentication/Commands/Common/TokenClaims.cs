namespace Overclocked.Application.Authentication.Commands.Common;

public record TokenClaims(
    string UserId,
    string Email,
    string DeviceId,
    string Role,
    IEnumerable<string> Permissions);
